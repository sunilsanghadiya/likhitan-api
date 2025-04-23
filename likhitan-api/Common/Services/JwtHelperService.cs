using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace likhitan.Common.Services
{
    public class JwtHelperService
    {
        public IConfiguration _configuration { get; set; }

        public JwtHelperService(IConfiguration configuration) 
        {
            _configuration = configuration;
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

        public string GenerateJwtToken(string username)
        {
            var key = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Jwt:key"));
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString().Replace("-", ""))
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

                if (validatedToken is JwtSecurityToken jwtToken &&
                    jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
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

        public string GenerateRefreshToken(string username, TimeSpan expiry, string secretKey)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString().Replace("-", ""))
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

    }
}
