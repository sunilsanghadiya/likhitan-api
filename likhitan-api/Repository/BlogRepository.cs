using likhitan.Db;
using likhitan.Entities;
using Microsoft.EntityFrameworkCore;

namespace likhitan_api.Repository
{
    public interface IBlogRepository
    {
        Task SaveBlog(Blogs blog);
        Task UpdateBlog(Blogs blog);
        Task<Blogs?> GetBlogById(int id);
        Task<bool> IsBlogExists(int id);

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
            if(blog.Id > 0)
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
    }
}
