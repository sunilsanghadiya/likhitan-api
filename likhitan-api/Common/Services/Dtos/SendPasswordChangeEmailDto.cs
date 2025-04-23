namespace likhitan.Common.Services.Dtos
{
    public class SendPasswordChangeEmailDto
    {
        public string Email { get; set; }
        public string ForwardedIp { get; set; }
        public string RealIp { get; set; }
        public string UserIp { get; set; }
        public string UserAgent { get; set; }
    }
}
