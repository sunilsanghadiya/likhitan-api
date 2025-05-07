using System.Linq;
using likhitan.Db;
using likhitan.Entities;
using likhitan_api.Models;
using likhitan_api.Models.ClientDto;
using Microsoft.EntityFrameworkCore;

namespace likhitan_api.Repository
{
    public interface IBlogRepository
    {
        Task SaveBlog(Blogs blog);
        Task UpdateBlog(Blogs blog);
        Task<Blogs?> GetBlogById(int id);
        Task<bool> IsBlogExists(int id);
        IQueryable<BlogsResponse> GetBlogs(GetBlogsDto getBlogsDto);

    }
    public class BlogRepository : IBlogRepository
    {
        private ApplicationDbContext _context;

        public BlogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveBlog(Blogs blog)
        {
            _context.Add(blog);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBlog(Blogs blog)
        {
            if (blog.Id > 0)
            {
                _context.Update(blog);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Blogs?> GetBlogById(int id) =>
            await _context.Blogs.AsNoTracking().Where(b => b.Id == id && !b.IsDeleted).Select(d => new Blogs
            {
                Id = d.Id,
                AuthorId = d.AuthorId,
                Title = d.Title,
                Slug = d.Slug,
                Content = d.Content,
                Excerpt = d.Excerpt,
                CategoryId = d.CategoryId,
                TagId = d.TagId,
                ThumbnailUrl = d.ThumbnailUrl,
                IsDeleted = d.IsDeleted
            }).FirstOrDefaultAsync();

        public async Task<bool> IsBlogExists(int id) =>
            await _context.Blogs.AsNoTracking().AnyAsync(a => a.Id == id);

        public IQueryable<BlogsResponse> GetBlogs(GetBlogsDto getBlogsDto)
        {
            return _context.Author
                .GroupJoin(_context.Blogs, a => a.Id, b => b.AuthorId, (a, blogs) => new { a, blogs })
                .SelectMany(ab => ab.blogs.DefaultIfEmpty(), (ab, b) => new { ab.a, Blog = b })
                .GroupJoin(_context.BlogLikes, ab => ab.Blog.Id, bl => bl.BlogId, (ab, likes) => new { ab.a, ab.Blog, likes })
                .GroupJoin(_context.BlogComments, ab => ab.Blog.Id, bc => bc.BlogId, (ab, comments) => new { ab.a, ab.Blog, ab.likes, comments })
                .GroupJoin(_context.BlogViews, ab => ab.Blog.Id, bv => bv.BlogId, (ab, views) => new { ab.a, ab.Blog, ab.likes, ab.comments, views })
                .Select(result => new BlogsResponse
                {
                    AuthorId = result.a.Id,
                    BlogId = result.Blog.Id,
                    Content = result.Blog.Content,
                    ThumbnailUrl = result.Blog.ThumbnailUrl,
                    Title = result.Blog.Title,
                    Slug = result.Blog.Slug,
                    LikeCount = result.likes.Select(x => x.Id).Distinct().Count(),
                    CommentCount = result.comments.Select(x => x.Id).Distinct().Count(),
                    ViewCount = result.views.Select(x => x.Id).Distinct().Count(),
                    Comments = result.comments
                        .OrderByDescending(c => c.Created)
                        .Take(10)
                        .Select(c => new BlogComments
                        {
                            Id = c.Id,
                            Comment = c.Comment,
                            UserId = c.UserId,
                            Created = c.Created
                        }).ToList()
                })
                .Skip((getBlogsDto.PageNumber - 1) * getBlogsDto.PageSize)
                .Take(getBlogsDto.PageSize);
        }
    }
}
