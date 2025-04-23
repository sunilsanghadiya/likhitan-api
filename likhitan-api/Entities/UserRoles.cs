namespace likhitan.Entities
{
    public class UserRoles
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
