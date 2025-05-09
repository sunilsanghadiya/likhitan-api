using likhitan_api.Repository;
using likhitan_api.SignalRHubs.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace likhitan_api.SignalRHubs
{
    [Authorize]
    public sealed class BlogHub : Hub
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IBlogLikesRepository _blogLikesRepository;
        private readonly IBlogCommentsRepository _blogCommentsRepository;

        public BlogHub(IBlogRepository blogRepository) 
        {
            _blogRepository = blogRepository;
        }

        public async Task LikeBlog(int blogId, int userId, string displayName)
        {
            await _blogLikesRepository.LikeBlogAsync(blogId, userId);
            await Clients.All.SendAsync("BlogLiked", blogId, displayName);
        }

        public async Task UnlikeBlog(int blogId, string displayName, int userId)
        {
            //var result = await _blogLikesRepository.UnlikeBlogAsync(blogId, userId);

            //if (result)
            //{
            //    await Clients.All.SendAsync("BlogUnliked", blogId, displayName);
            //}
        }

        public async Task CreateBlog(CreateBlogSignalRDto createBlogSignalRDto)
        {

            createBlogSignalRDto.AuthorId = createBlogSignalRDto.UserId ?? 0;
            createBlogSignalRDto.Created = DateTime.UtcNow;

            //var createdBlog = await _blogRepository.CreateBlogAsync(createBlogSignalRDto);

            //await Clients.All.SendAsync("NewBlogCreated", createdBlog.Id, createdBlog.Title, createBlogSignalRDto.UserDisplayName ?? "");
        }

        public async Task SaveBlog(int blogId, int userId)
        {
            //await _blogRepository.SaveBlogAsync(blogId, userId);
            //await Clients.User(userId.ToString()).SendAsync("BlogSaved", blogId);
        }

        public async Task UnsaveBlog(int blogId, int userId)
        {
            //var result = await _blogRepository.UnsaveBlogAsync(blogId, userId);

            //if (result)
            //{
            //    await Clients.User(userId.ToString()).SendAsync("BlogUnsaved", blogId);
            //}
        }

        public async Task SaveComment(SaveCommentSignalRDto saveCommentSignalRDto)
        {
            //var result = await _blogCommentsRepository.SaveCommentAsync(saveCommentSignalRDto);
            //if (result)
            //{
            //    await Clients.User(saveCommentSignalRDto.UserId.ToString()).SendAsync("SaveComment", saveCommentSignalRDto.BlogsId);
            //}
        }

        public async Task EditComment(int commentId, int userId)
        {
            //var result = await _blogCommentsRepository.EditCommentAsync(commentId, userId);
            //if (result)
            //{
            //    await Clients.User(userId.ToString()).SendAsync("EditComment", commentId);
            //}
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "BlogUsers");
            await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "BlogUsers");
            await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
