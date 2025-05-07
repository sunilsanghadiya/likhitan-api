using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure.Core;
using Microsoft.IdentityModel.Tokens;

namespace likhitan.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        public IConfiguration Configuration { get; set; }
        private const string SecretKey = "962b6e8b039403139dc5d32f5f3f618bc1de10bb8d5945d6bf45d143b91de30435b73cb5e411275418098ab4dbd4c55d637e2bb3291c976cfb3c84f024ae9aa0dcfbf6167610418b6df33faaa45fc4e11d16b8f183731ef9d6f4e84c0af02813c6d2b84769d56897fc7ae5db49f56767a381f0342b9feed34216508006ab935c00d371074644898c884091f70ebcc486a99c7a63c889d70a6c0b9de5125b1ddf5d1b2c698fb9f5962ff211d43ebcbaddd2d63d2f88f5dbd3735a27797a8ae762bf96ac766b15f6662fbfd7c2fd6307bdc98fae9326ec0a1a0cd5a7d392eb2f958c9f3fca28472d79f0f5eee2a507df9435625cc059e81aad155724fd34535ef5741fea927ed62f3705fb4ad95f22c149d1fce0e14fda952133bc7526088f124e659802570036732d6dd584e648d57943e6d3b2c702e49d93a163c878afa8a7e9429f3384322567c9f1c3011bc86ae8d8462acd1c691ce92d609868a1633171a44a5288af7b59e448d04e089a3b2ac6d0d22c5d5d68fa7019c04502ddce1accd1caa63c10d47a8c2723e5b298560314d1b31932cfa7a9058532175ab8a4393214938a7643ef7ca98a6d1c8235d57dbd8cfb9351ac3fef57a73dd3cf5e625e94d69f5d5c32e69585e39eba9d46297b3a7f7dfc9535800ace1bf84cb291df97371dbee9f202f2c5243216d5efa692ad645e733a4a41130aacc1a57eff226fee52cc";

        public AuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            Configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;

            var publicRoutes = new[] {
                "/api/auth/login",
                "/api/auth/isEmailExists",
                "/api/auth/register",
                "/api/auth/sendOTP",
                "/",
                "/api/auth/forgotpassword",
                "/api/auth/refreshToken",
                "/api/auth/RegisterWithOAuth",
                "api/validate/isUserAuthenticated",
                "/api/auth/IsEmailDomainSupport",
                "/api/auth/GetOTP"
            };

            if (publicRoutes.Contains<string>(path))
            {
                await _next(context);
                return;
            }
           
            var token = context.Request.Cookies["AccessToken"];
            var refreshToken = context.Request.Cookies["RefreshToken"];

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshToken))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            var principal = ValidateToken(token);

            if (principal == null)
            {
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var newTokens = RefreshTokens(refreshToken);
                    if (newTokens != null)
                    {
                        SetCookies(context, newTokens.Value.AccessToken, newTokens.Value.RefreshToken);
                        context.Items["User"] = ValidateToken(newTokens.Value.AccessToken)?.Claims;
                        await _next(context);
                        return;
                    }
                }

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid or Expired Token");
                return;
            }

            context.Items["User"] = principal.Claims;
            await _next(context);
        }

        private ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]);

                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "https://localhost:7043",
                    ValidAudience = "https://localhost",
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                var principal = tokenHandler.ValidateToken(token, parameters, out _);
                return principal;
            }
            catch (SecurityTokenExpiredException)
            {
                return null; // Expired
            }
            catch
            {
                return null; // Invalidtoken
            }
        }

        private (string AccessToken, string RefreshToken)? RefreshTokens(string refreshToken)
        {
            var principal = ValidateToken(refreshToken);
            if (principal == null) return null;

            var newAccessToken = GenerateToken(principal.Claims, TimeSpan.FromMinutes(15));

            var newRefreshToken = GenerateToken(principal.Claims, TimeSpan.FromDays(7));

            return (newAccessToken, newRefreshToken);
        }

        private string GenerateToken(IEnumerable<Claim> claims, TimeSpan expiry)
        {
            var key = Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(expiry),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "https://localhost:7043",
                Audience = "https://localhost"
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private void SetCookies(HttpContext context, string accessToken, string refreshToken)
        {
            var accessCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(15)
            };

            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            context.Response.Cookies.Append("AccessToken", accessToken, accessCookieOptions);
            context.Response.Cookies.Append("RefreshToken", refreshToken, refreshCookieOptions);
        }
    }
}
