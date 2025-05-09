using System.Text;
using likhitan.Common.Services;
using likhitan.Db;
using likhitan.Middlewares;
using likhitan.Repository;
using likhitan.Services;
using likhitan_api.Common.Services;
using likhitan_api.Repository;
using likhitan_api.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace likhitan
{
    public class Startup
    {
        public Startup(
            IConfiguration configuration
            //, IWebHostEnvironment environment
            )
        {
            Configuration = configuration;
            //Environment = environment;
        }

        private IConfiguration Configuration { get; }
        //private IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.WithOrigins("https://localhost:4200") 
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials(); 
                });
            });

            services.AddControllers();
            services.AddMemoryCache();

            services.AddHttpContextAccessor();
            services.AddHttpClient<IDisposableEmailChecker, DisposableEmailChecker>();

            services.AddAuthentication(
                options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; // Default to Google
            }
            )
            .AddCookie();
            //.AddGoogle(googleOptions =>
            //{
            //    googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
            //    googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            //    googleOptions.SaveTokens = true;
            //})
            //.AddFacebook(facebookOptions =>
            //{
            //    facebookOptions.ClientId = Configuration["Authentication:Facebook:ClientId"];
            //    facebookOptions.ClientSecret = Configuration["Authentication:Facebook:ClientSecret"];
            //    facebookOptions.SaveTokens = true;
            //})
            //.AddTwitter(twitterOptions =>
            //{
            //    twitterOptions.ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"];
            //    twitterOptions.ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"];
            //    twitterOptions.RetrieveUserDetails = true;
            //});

            services.AddAuthorization();

            services.AddAutoMapper(typeof(AutoMapperProfile));

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<JwtHelperService>();

            #region JWT
            var key = Configuration["Jwt:Key"];
            var issuer = Configuration["Jwt:Issuer"];
            var audience = Configuration["Jwt:Audience"];

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    //options.Authority = "https://your-oauth-provider.com";  // OAuth provider URL
                    //options.Audience = "https://localhost7043";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                        ClockSkew = TimeSpan.Zero
                    };

                    // Read token from cookie
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var token = context.Request.Cookies["AccessToken"]; // Read JWT from HttpOnly cookie
                            //var refreshToken = context.Request.Cookies["RefreshToken"]; // Read JWT from HttpOnly cookie
                            if (!string.IsNullOrEmpty(token))
                            {
                                context.Token = token;
                                //context.Token = refreshToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
            #endregion

            #region Rate Limit
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("FixtedPolicy", policy =>
                {
                    policy.PermitLimit = 5;
                    policy.Window = TimeSpan.FromSeconds(10);
                    policy.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                    policy.QueueLimit = 2;
                });

                options.AddConcurrencyLimiter("ConcurrencyPolicy", policy =>
                {
                    policy.PermitLimit = 3;
                    policy.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                    policy.QueueLimit = 5;
                });
            });
            #endregion

            #region Reddis
            var redisConfig = Configuration.GetSection("Redis");
            string connectionString = $"{redisConfig["Host"]}:{redisConfig["Port"]},password={redisConfig["Password"]}";
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(connectionString));
            #endregion

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "Likhitan API",
                    Description = "This is the likhitan api app",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Your Name",
                        Email = "your.email@example.com",
                        Url = new Uri("https://localhost:7043")
                    }
                });
            });

            services.AddHealthChecks();

            //services.AddHostedService<RedisCleanupService>();
            services.AddScoped<RedisService>();
            services.AddTransient<EmailService>();

            #region Business Logic Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IUserTrackingRepository, UserTrackingRepository>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IBlogRepository, BlogRepository>();
            services.AddScoped<IBlogCommentsRepository, BlogCommentsRepository>();
            #endregion

            #region Business Logic Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IUserTrackingService, UserTrackingService>();
            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IBlogCommentsService, BlogCommentsService>();
            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("CorsPolicy");

            app.UseMiddleware<AuthMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<CustomCookieMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints((endpoints) =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
