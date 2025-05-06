using System.ComponentModel.DataAnnotations;

namespace likhitan.Entities
{
    public class BlogViews
    {
        [Key]
        public int Id { get; set; }
        public int BlogId {  get; set; }
        public int UserId { get; set; }
        public string? IpAddress { get; set; }
        public DateTime? Viewed { get; set; }
        public User User { get; set; }
        public Blogs Blogs { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? Deleted { get; set; }
    }
}
