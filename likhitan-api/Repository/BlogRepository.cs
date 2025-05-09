using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using likhitan.Db;
using likhitan.Entities;
using likhitan_api.Common.Repository;
using likhitan_api.Common.Services;
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
        bool IsBlogExists(int id);
        IQueryable<BlogsResponse> GetBlogs(GetBlogsDto getBlogsDto);

    }
    public class BlogRepository : BaseRepository<Blogs>, IBlogRepository
    {
        protected override Expression<Func<Blogs, dynamic>>[] Includes =>
        [
            a => a.BlogCategory,
            a => a.BlogViews,
            a => a.BlogComments,
            a => a.BlogLikes
        ];

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

        public bool IsBlogExists(int id) =>
             _context.Blogs.AsNoTracking().Any(a => a.Id == id);

        public IQueryable<BlogsResponse> GetBlogs(GetBlogsDto getBlogsDto)
        {
            return _context.Blogs
                    .AsNoTracking()
                    .OrderBy(b => b.Created)
                    .Skip((getBlogsDto.PageNumber - 1) * getBlogsDto.PageSize)
                    .Take(getBlogsDto.PageSize)
                    .Select(b => new BlogsResponse
                    {
                        BlogId = b.Id,
                        AuthorId = b.AuthorId,
                        Published = b.Published,
                        Title = b.Title,
                        Content = b.Content,
                        Slug = b.Slug,
                        ThumbnailUrl = b.ThumbnailUrl,
                        LikeCount = _context.BlogLikes.Where(lc => lc.BlogId == b.Id).Count(),
                        CommentCount = _context.BlogComments.Where(bc => bc.BlogsId == b.Id).Count(),
                        ViewCount = _context.BlogViews.Where(bv => bv.BlogId == b.Id).Count(),
                        BlogComments = _context.BlogComments.Where(c => c.BlogsId == b.Id).ToList(),
                    });
        }
    }
}

