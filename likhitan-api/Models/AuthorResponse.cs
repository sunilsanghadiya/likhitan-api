namespace likhitan_api.Models
{
    public class AuthorResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
