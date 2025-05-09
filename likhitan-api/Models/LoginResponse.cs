namespace likhitan.Models
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsUserVerified { get; set; }
        public int RoleId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int AuthorId { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? ImageUrl { get; set; }
    }
}
