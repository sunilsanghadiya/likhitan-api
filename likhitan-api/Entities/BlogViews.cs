namespace likhitan.Entities
{
    public class BlogViews
    {
        public Guid Id { get; set; }
        public Guid BlogId {  get; set; }
        public int UserId { get; set; }
        public string IpAddress { get; set; }
        public DateTime? Viewed { get; set; }
        public ICollection<User> User { get; set; }
        public ICollection<Blogs> Blogs { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime Deleted { get; set; }
    }
}
