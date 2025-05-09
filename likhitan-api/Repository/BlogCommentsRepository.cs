using likhitan.Db;
using likhitan.Entities;
using Microsoft.EntityFrameworkCore;

namespace likhitan_api.Repository
{
    public interface IBlogCommentsRepository
    {
        Task SaveComment(BlogComments blogComments);
        IQueryable<BlogComments> GetUserCommentsByBlogId(int blogId);
    }
    public class BlogCommentsRepository : IBlogCommentsRepository
    {
        private readonly ApplicationDbContext _context;

        public BlogCommentsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveComment(BlogComments  blogComments)
        {
            if(blogComments.Id <= 0)
            {
                _context.BlogComments.Add(blogComments);
                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<BlogComments> GetUserCommentsByBlogId(int blogId)
        {
            return _context.BlogComments.AsNoTracking().Where(b => b.BlogsId == blogId && !b.IsDeleted && b.IsActive).OrderByDescending(c => c.Created);
        }

    }
}
