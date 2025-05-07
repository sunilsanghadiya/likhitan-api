namespace likhitan.Entities
{
    public class Tags
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? Deleted { get; set; }
        public virtual ICollection<Blogs> Blogs { get; set; } = [];
    }
}
