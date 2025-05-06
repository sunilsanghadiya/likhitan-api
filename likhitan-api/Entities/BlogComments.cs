using System.ComponentModel.DataAnnotations;

namespace likhitan.Entities
{
    public class BlogComments
    {
        [Key]
        public int Id { get; set; }
        public int BlogId { get; set; }
        public string Comment { get; set; }
        public int UserId { get; set; }
        public int ParentId { get; set; } //for self reference nested comment
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? Deleted { get; set; }
        public Blogs Blogs { get; set; }
        public DateTime Created { get; set; }
        public User User { get; set; }
    }
}
