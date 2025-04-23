namespace likhitan.Models
{
    public class SendOTPResponse
    {
        public bool IsOtpSend { get; set; } = false;
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
