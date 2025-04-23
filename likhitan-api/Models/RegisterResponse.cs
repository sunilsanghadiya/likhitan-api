namespace likhitan.Models
{
    public class RegisterResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsUserVerified { get; set; }
        public int RoleId { get; set; }
        public DateTime Created { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public bool IsTeamsAndConditionAccepted { get; set; }
    }
}
