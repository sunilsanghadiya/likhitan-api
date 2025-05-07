namespace likhitan_api.Models
{
    public class GetOTPResponse
    {
        public string OTP {  get; set; }
        public DateTime ExpiredDate { get; set; }
    }
}
