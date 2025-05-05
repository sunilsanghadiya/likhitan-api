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

            var blog = new Blogs()
            {
                Id = 0,
                Title = writeBlogDto.Title,
                Content = writeBlogDto.Content,
                IsDeleted = false,
                Created = DateTime.UtcNow,
                AuthorId = writeBlogDto?.AuthorId ?? 0,
            };
            await _blogRepository.SaveBlog(blog);

            var writedBlog = await _blogRepository.IsBlogExists(blog.Id);

            WriteBlogResponse writeBlogResponse = new()
            {
                IsBlogPosted = writedBlog,
            };

            return Result<WriteBlogResponse>.Success(writeBlogResponse);
        }
    }
}
