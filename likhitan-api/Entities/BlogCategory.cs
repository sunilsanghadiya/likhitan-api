namespace likhitan.Entities
{
    public class BlogCategory
    {
        public int Id { get; set; }
        public Guid BlogId { get; set; }
        public DateTime Created { get; set; }
        public ICollection<Blogs> Blogs { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime Deleted { get; set; }
    }
}
