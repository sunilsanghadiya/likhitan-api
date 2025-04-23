using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using likhitan.Common.Services;
using Microsoft.IdentityModel.Tokens;

namespace likhitan.Middlewares
{
    public class CustomCookieMiddleware
    {
        private readonly RequestDelegate _next;
        public IConfiguration Configuration { get; set; }

        public CustomCookieMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            Configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var redisService = context.RequestServices.GetRequiredService<RedisService>();
            var token = context.Request.Cookies["AccessToken"];
            var emailId = ValidateToken(token ?? "");
            if (!string.IsNullOrEmpty("emailId"))
            {
                var cookies = context.Request.Cookies["AccessToken"];

                await redisService.SetValueAsync("emailId", cookies ?? "EMPTY_COOKIE");
            }

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
    }

}
