namespace likhitan.Models.ClientDto
{
    public class UserTrackingDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime LoginTime { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
