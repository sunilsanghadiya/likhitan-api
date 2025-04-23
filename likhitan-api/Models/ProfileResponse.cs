using likhitan.Entities;

namespace likhitan.Models
{
    public class ProfileResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public bool IsUserVerified { get; set; }
        public int RoleId { get; set; }
        public DateTime Created { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? Deleted { get; set; }

    }
}
