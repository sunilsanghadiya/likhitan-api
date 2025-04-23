using likhitan.Db;
using likhitan.Entities;
using Microsoft.EntityFrameworkCore;

namespace likhitan.Repository
{
    public interface IUserTrackingRepository
    {
        Task<UserTracking> GetUserTrackingById(int userId);
        Task<UserTracking> SaveUserTracking(UserTracking userTracking);
    }
    public class UserTrackingRepository : IUserTrackingRepository
    {
        private readonly ApplicationDbContext _context;

        public UserTrackingRepository(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task<UserTracking> GetUserTrackingById(int userId) =>
              await _context.UserTracking.FirstOrDefaultAsync(a => a.UserId == userId);

        public async Task<UserTracking> SaveUserTracking(UserTracking userTracking)
        {
            _context.UserTracking.Add(userTracking);
            await _context.SaveChangesAsync();
            return userTracking;
        }
    }
}
