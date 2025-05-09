using AutoMapper;
using likhitan.Entities;
using likhitan_api.Models.ClientDto;
using likhitan_api.Repository;

namespace likhitan_api.Services
{
    public interface IBlogCommentsService
    {
        Task SaveBlogComment(BlogCommentDto blogCommentDto);
    }
    public class BlogCommentsService : IBlogCommentsService
    {
        private readonly IBlogCommentsRepository _blogCommentsRepository;
        private readonly IMapper _mapper;

        public BlogCommentsService(IBlogCommentsRepository blogCommentsRepository, IMapper mapper) 
        {
            _blogCommentsRepository = blogCommentsRepository;
            _mapper = mapper;
        }

        public async Task SaveBlogComment(BlogCommentDto blogCommentDto)
        {
            if(blogCommentDto.Id == 0)
            {
                var mappedBlogComments = _mapper.Map<BlogComments>(blogCommentDto);
                await _blogCommentsRepository.SaveComment(mappedBlogComments);
            }
        }
    }
}
