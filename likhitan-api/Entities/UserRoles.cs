using System.ComponentModel.DataAnnotations;

namespace likhitan.Entities
{
    public class UserRoles
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? Deleted { get; set; }
        public virtual ICollection<User> Users { get; set; } = [];
    }
}
