using likhitan_api.Common.Enums;

namespace likhitan_api.Models.ClientDto
{
    public class BlogActionDto
    {
        public int LoggedInUserId { get; set; }
        public int BlogId { get; set; }
        public BlogActionTypes ActionType { get; set; }
        public string? Comment { get; set; }
        public int ParentId { get; set; } = 0;
    }
}
