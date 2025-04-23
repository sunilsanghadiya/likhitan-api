namespace likhitan.Entities
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public ICollection<User> User { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime Deleted { get; set; }
        public DateTime Created { get; set; }
    }
}
