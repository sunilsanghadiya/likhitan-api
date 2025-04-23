namespace likhitan.Entities
{
    public class Blogs
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Slug { get; set; }
        public string Content { get; set; }
        public string? Excerpt { get; set; }
        public int AuthorId { get; set; }
        public int CategoryId { get; set; }
        public int TagId { get; set; }
        public string ThumbnailUrl { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime Deleted { get; set; }
        public Author Author { get; set; }
        public BlogCategory BlogCategory { get; set; }
        public Tags Tags { get; set; } 
    }
}
