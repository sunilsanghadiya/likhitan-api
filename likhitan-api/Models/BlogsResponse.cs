using likhitan.Entities;

namespace likhitan_api.Models
{
    public class BlogsResponse
    {
        public int AuthorId { get; set; }
        public bool IsActive { get; set; }
        public int BlogId { get; set; }
        public string Content { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public int ViewCount { get; set; }
        public List<BlogComments> BlogComments { get; set; } = [];
        public List<BlogViews> BlogViews { get; set; } = [];
        public List<BlogLikes> BlogLikes { get; set; } = [];
        public DateTime? Published { get; set; }
    }
}
