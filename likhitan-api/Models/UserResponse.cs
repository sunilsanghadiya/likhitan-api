using likhitan.Entities;

namespace likhitan.Models
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public bool IsUserVerified { get; set; }
        public int RoleId { get; set; }
        public UserRoles Role { get; set; }
        public DateTime Created { get; set; }
        public string OTP { get; set; }
        public DateTime OTPExpire { get; set; }
        public int AuthorId { get; set; }
    }
}
