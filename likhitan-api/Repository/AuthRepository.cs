using likhitan.Db;
using likhitan.Entities;
using likhitan.Models.ClientDto;
using Microsoft.EntityFrameworkCore;

namespace likhitan.Repository
{
    public interface IAuthRepository
    {
        Task<User?> Login(LoginDto user);
    }
    public class AuthRepository : IAuthRepository
    {
        public readonly ApplicationDbContext _context;
        public AuthRepository(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task<User?> Login(LoginDto user)
        {
            return await _context.Users.FirstOrDefaultAsync(u =>
                    (u.Email == user.Email) &&
                    u.IsActive &&
                    u.IsUserVerified &&
                    !u.IsDeleted);
        }
    }
}
