using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace likhitan.Common.Services
{
    public class JwtHelperService
    {
        public IConfiguration _configuration { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtHelperService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor) 
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
           
        }

        public ClaimsPrincipal? GetClaimsFromToken(string token, string? signingKey = null)
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
                return null;

            var jwtToken = handler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");

            return new ClaimsPrincipal(identity);
        }

        public string? GetClaimValue(string token, string claimType)
        {
            var claimsPrincipal = GetClaimsFromToken(token);
            return claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        }

        public string GenerateJwtToken(string username, string role)
        {
            var key = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Jwt:key"));
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString().Replace("-", "")),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: "https://localhost:7043",
                audience: "https://localhost",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpiresInMinutes")),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal ValidateToken(string token, string secretKey, bool validateLifetime = true)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = validateLifetime,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    return principal;
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        public string GenerateRefreshToken(string username, TimeSpan expiry, string secretKey, string role)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString().Replace("-", "")),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "https://localhost:7043",
                audience: "https://localhost",
                claims: claims,
                expires: DateTime.UtcNow.Add(expiry),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateToken(IEnumerable<Claim> claims, TimeSpan expiry, string secretKey)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "https://localhost:7043",
                audience: "https://localhost",
                claims: claims,
                expires: DateTime.UtcNow.Add(expiry),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ReadJwtToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var keyBytes = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Jwt:Key"));

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ValidateIssuer = false, // Set to true if you have a specific issuer
                    ValidateAudience = false, // Set to true if you have a specific audience
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Prevents expired tokens from being accepted within a grace period
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (Exception)
            {
                return null; // Token is invalid
            }
        }

        public LoggedInUserDetails GetLoggedInUserDetails()
        {
            var accessToken = _httpContextAccessor.HttpContext?.Request?.Cookies["AccessToken"];
            var refreshToken = _httpContextAccessor.HttpContext?.Request?.Cookies["RefreshToken"];
            var claimedResult = ValidateToken(accessToken ?? "", "962b6e8b039403139dc5d32f5f3f618bc1de10bb8d5945d6bf45d143b91de30435b73cb5e411275418098ab4dbd4c55d637e2bb3291c976cfb3c84f024ae9aa0dcfbf6167610418b6df33faaa45fc4e11d16b8f183731ef9d6f4e84c0af02813c6d2b84769d56897fc7ae5db49f56767a381f0342b9feed34216508006ab935c00d371074644898c884091f70ebcc486a99c7a63c889d70a6c0b9de5125b1ddf5d1b2c698fb9f5962ff211d43ebcbaddd2d63d2f88f5dbd3735a27797a8ae762bf96ac766b15f6662fbfd7c2fd6307bdc98fae9326ec0a1a0cd5a7d392eb2f958c9f3fca28472d79f0f5eee2a507df9435625cc059e81aad155724fd34535ef5741fea927ed62f3705fb4ad95f22c149d1fce0e14fda952133bc7526088f124e659802570036732d6dd584e648d57943e6d3b2c702e49d93a163c878afa8a7e9429f3384322567c9f1c3011bc86ae8d8462acd1c691ce92d609868a1633171a44a5288af7b59e448d04e089a3b2ac6d0d22c5d5d68fa7019c04502ddce1accd1caa63c10d47a8c2723e5b298560314d1b31932cfa7a9058532175ab8a4393214938a7643ef7ca98a6d1c8235d57dbd8cfb9351ac3fef57a73dd3cf5e625e94d69f5d5c32e69585e39eba9d46297b3a7f7dfc9535800ace1bf84cb291df97371dbee9f202f2c5243216d5efa692ad645e733a4a41130aacc1a57eff226fee52cc");

            return new LoggedInUserDetails()
            {
                Email = claimedResult.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? "",
                RoleId = claimedResult.Claims.FirstOrDefault(r => r.Type == ClaimTypes.Role)?.Value ?? "",
                AccessToken = accessToken ?? "",
                RefreshToken = refreshToken ?? ""
            };
        }
    }

    public class LoggedInUserDetails
    {
        public string Email { get; set; }
        public string RoleId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

    }
}
