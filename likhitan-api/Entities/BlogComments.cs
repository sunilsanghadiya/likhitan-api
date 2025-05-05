namespace likhitan.Entities
{
    public class BlogComments
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        public string Comment { get; set; }
        public int UserId { get; set; }
        public int ParentId { get; set; } //for self reference nested comment
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime Deleted { get; set; }
        public ICollection<Blogs> Blogs { get; set; }
        public DateTime Created { get; set; }
        public ICollection<User> User { get; set; }
    }
}
