using likhitan.Db;
using likhitan.Entities;
using Microsoft.EntityFrameworkCore;

namespace likhitan.Repository
{
    public interface IUserRepository
    {
        Task<User> SaveUser(User user);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserById(int id);
        Task<List<string>?> GetUserByEmailId(string email);
        Task<User?> GetSimpleUserById(int id);
        Task<User?> GetUserDetailByEmailId(string email);
    }
    public class UserRepository : IUserRepository
    {
        public readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> SaveUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetUserByEmail(string email) =>
            await _context.Users.FirstOrDefaultAsync(a => a.Email == email && !a.IsDeleted);

        public async Task<User?> GetUserById(int id) =>
            await _context.Users.FirstOrDefaultAsync(a => a.Id == id && a.IsActive && a.IsUserVerified && !a.IsDeleted);

        public async Task<User?> GetSimpleUserById(int id) =>
            await _context.Users.Where(a => a.Id == id && !a.IsDeleted)?.Select(v => new User
            {
                OTP = v.OTP, 
                OTPExpire = v.OTPExpire, 
                Id = v.Id, 
                Email = v.Email
            }).FirstOrDefaultAsync();

        public async Task<User?> GetUserDetailByEmailId(string email) =>
            await _context.Users.Where(a => a.Email == email).Select(u => new User 
            { 
                Id = u.Id,
                Name = u.Name,
            }).FirstOrDefaultAsync();

        public async Task<List<string>?> GetUserByEmailId(string email) =>
            await _context.Users.Where(a => a.Email == email && a.IsActive).Select(a => a.Email).ToListAsync();

    }
}
