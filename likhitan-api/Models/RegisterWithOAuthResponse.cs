using System.Security.Claims;

namespace likhitan.Models
{
    public class RegisterWithOAuthResponse
    {
        public string Provider { get; set; }
        public IEnumerable<Claim> User { get; set; }
        public string AccessToken { get; set; }
    }
}
