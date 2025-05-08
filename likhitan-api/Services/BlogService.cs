using AutoMapper;
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
        Task<Result<IEnumerable<BlogsResponse>>> GetBlogs(GetBlogsDto getBlogsDto);
    }
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;
        private readonly JwtHelperService _jwtHelperService;
        private IMapper _mapper;

        public BlogService(IBlogRepository blogRepository, JwtHelperService jwtHelperService, IMapper mapper)
        {
            _blogRepository = blogRepository;
            _jwtHelperService = jwtHelperService;
            _mapper = mapper;
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
                IsPublished = true,
                Published = DateTime.UtcNow
            };
            await _blogRepository.SaveBlog(blog);

            var writedBlog = _blogRepository.IsBlogExists(blog.Id);

            WriteBlogResponse writeBlogResponse = new()
            {
                IsBlogPosted = writedBlog,
            };

            return Result<WriteBlogResponse>.Success(writeBlogResponse);
        }

        public async Task<Result<IEnumerable<BlogsResponse>>> GetBlogs(GetBlogsDto getBlogsDto)
        {
            //var loggedInUserDetails = _jwtHelperService.GetLoggedInUserDetails();
            var blogs = _blogRepository.GetBlogs(getBlogsDto);
            return Result<IEnumerable<BlogsResponse>>.Success(blogs);
        }
    }
}
