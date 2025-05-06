namespace likhitan_api.Entities
{
    public abstract class BaseEntity
    {
        public bool IsDeleted { get; protected set; } = false;
        public DateTime? Deleted { get; protected set; }
        public bool IsActive { get; protected set; }
        public DateTime Created { get; protected set; }
        public DateTime? Updated { get; protected set; }


        protected BaseEntity() 
        {
            Created = DateTime.UtcNow;
        }
    }
}
