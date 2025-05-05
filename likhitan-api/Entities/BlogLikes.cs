namespace likhitan.Entities
{
    public class BlogLikes
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        public int UserId { get; set; }
        public ICollection<User> User { get; set; }
        public ICollection<Blogs> Blog { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime Deleted { get; set; }
        public DateTime Created { get; set; }
    }
}
