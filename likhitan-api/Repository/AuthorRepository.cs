using likhitan.Db;
using likhitan.Entities;
using Microsoft.EntityFrameworkCore;

namespace likhitan.Repository
{
    public interface IAuthorRepository
    {
        Task SaveBecomeAuthor(Author author);
        Task<Author?> GetAuthorById(int id);
    }
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ApplicationDbContext _context;
        public AuthorRepository(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task SaveBecomeAuthor(Author author)
        {
            await _context.AddAsync(author);
            await _context.SaveChangesAsync();
        }

        public async Task<Author?> GetAuthorById(int id) =>
             await _context.Author.FirstOrDefaultAsync(a => a.Id == id);
    }
}
