using System.ComponentModel.DataAnnotations;

namespace likhitan.Entities
{
    public class Blogs
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Slug { get; set; }
        public string Content { get; set; }
        public string? Excerpt { get; set; }
        public int AuthorId { get; set; }
        public int? CategoryId { get; set; }
        public int? TagId { get; set; }
        public string? ThumbnailUrl { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? Deleted { get; set; }
        public Author Author { get; set; }
        public BlogCategory? BlogCategory { get; set; }
        public Tags? Tags { get; set; } 
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public virtual ICollection<BlogLikes> BlogLikes { get; set; } = [];
        public virtual ICollection<BlogComments> BlogComments { get; set; } = [];
        public virtual ICollection<BlogViews> BlogViews { get; set; } = [];
        public DateTime? Published { get; set; }
        public bool IsPublished { get; set; } = false;
    }
}
