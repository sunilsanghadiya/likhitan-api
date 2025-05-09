using likhitan.Entities;
using System.ComponentModel.DataAnnotations;

namespace likhitan_api.SignalRHubs.Dtos
{
    public class SaveCommentSignalRDto
    {
        [Key]
        public int Id { get; set; }
        public int BlogsId { get; set; }
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
