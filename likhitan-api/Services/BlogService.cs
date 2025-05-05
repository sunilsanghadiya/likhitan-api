using likhitan.Common.Services;
using likhitan.Entities;
using likhitan_api.Models;
using likhitan_api.Models.ClientDto;
using likhitan_api.Repository;

namespace likhitan_api.Services
{
    public interface IBlogService
    {
        Task<Result<WriteBlogResponse>> WriteBlog(WriteBlogDto writeBlogDto);
    }
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;

        public BlogService(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<Result<WriteBlogResponse>> WriteBlog(WriteBlogDto writeBlogDto)
        {
            if (string.IsNullOrEmpty(writeBlogDto.Title))
            {
                return Result<WriteBlogResponse>.BadRequest("please provide blog title");
            }
            if(string.IsNullOrEmpty(writeBlogDto.Content))
            {
                return Result<WriteBlogResponse>.BadRequest("please provide content");
            }

            var blogs = new Blogs()
            {
                Id = 0,
                Title = writeBlogDto.Title,
                Content = writeBlogDto.Content,

            };

            var writedBlog = await _blogRepository.IsBlogExists(blogs.Id);

            WriteBlogResponse writeBlogResponse = new()
            {
                IsBlogPosted = writedBlog ? true : false,
            };

            return Result<WriteBlogResponse>.Success(writeBlogResponse);
        }
    }
}
