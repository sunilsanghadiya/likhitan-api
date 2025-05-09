using AutoMapper;
using likhitan.Db;
using likhitan.Entities;

namespace likhitan_api.Repository
{
    public interface IBlogLikesRepository
    {
        #region SignalR Methods
        Task LikeBlogAsync(int blogId, int userId);
        Task UnlikeBlogAsync(int blogId, int userId);
        #endregion
    }
    public class BlogLikesRepository : IBlogLikesRepository
    {
        private readonly ApplicationDbContext _context;
        private IMapper _mapper;

        public BlogLikesRepository(ApplicationDbContext context)
        {
            _context = context;
        }



        #region SignalR Methods
        public async Task LikeBlogAsync(int blogId, int userId)
        {

        }
        public async Task UnlikeBlogAsync(int blogId, int userId)
        {

        }
        #endregion


    }
}
