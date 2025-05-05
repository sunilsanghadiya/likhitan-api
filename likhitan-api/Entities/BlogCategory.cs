namespace likhitan.Entities
{
    public class BlogCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime Deleted { get; set; }
    }
}
