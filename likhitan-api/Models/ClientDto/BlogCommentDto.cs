namespace likhitan_api.Models.ClientDto
{
    public class BlogCommentDto
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int UserId { get; set; }
        public int ParentId { get; set; }
        public int BlogsId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? Deleted { get; set; }
        public DateTime Created { get; set; }
    }
}
