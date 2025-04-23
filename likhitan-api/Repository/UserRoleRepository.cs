using likhitan.Db;
using likhitan.Entities;
using Microsoft.EntityFrameworkCore;

namespace likhitan.Repository
{
    public interface IUserRoleRepository
    {
        Task<UserRoles?> GetUserRoleById(int id);
    }
    public class UserRoleRepository : IUserRoleRepository
    { 
        public readonly ApplicationDbContext _context;
        public UserRoleRepository(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task<UserRoles?> GetUserRoleById(int id)
        {
            return await _context.UserRoles.FirstOrDefaultAsync(a => a.Id == id && a.IsActive);
        }
    }
}
