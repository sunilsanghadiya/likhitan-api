using System.Security.Claims;
using AutoMapper;
using likhitan.Common.Enums;
using likhitan.Common.Services;
using likhitan.Common.Services.Dtos;
using likhitan.Entities;
using likhitan.Models;
using likhitan.Models.ClientDto;
using likhitan.Repository;
using likhitan_api.Common.Services;
using likhitan_api.Models;
using likhitan_api.Models.ClientDto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace likhitan.Services
{
    public interface IAuthService
    {
        Task<Result<LoginResponse>> Login(LoginDto loginDto);
        Task<Result<RegisterResponse>> Register(RegisterDto registerDto);
        Task<Result<SendOTPResponse>> SendOTP(SendOtpDto sendOtpDto);
        Task<Result<EmailExitsResponse>> IsEmailExists(EmailExistsDto emailExistsDto);
        Task<Result<RefreshTokenResponse>> RefreshToken(HttpRequest request, HttpResponse response);
        Task<Result<ForgotPasswordResponse>> ForgotPassword(ForgotPasswordDto forgotPasswordDto);
        Task<Result<RegisterWithOAuthResponse>> RegisterWithOAuth(RegisterWithOAuthDto registerWithOAuthDto);
        Task<Result<LogoutResponse>> Logout(HttpRequest request, HttpResponse httpResponse);
        Task<Result<IsEmailDomainSupportResponse>> IsEmailDomainSupport(IsEmailDomainSupportDto obj);
    }
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        public IMapper _mapper;
        public readonly IHttpContextAccessor _httpContextAccessor;
        public IUserService _userService;
        public IConfiguration _configuration;
        public EmailService _emailService;
        private JwtHelperService _jwtHelperService;
        public IUserTrackingService _userTrackingService;
        public RedisService _redisService;
        public IWebHostEnvironment _env;
        private readonly IDisposableEmailChecker _emailChecker;

        public AuthService(
            IAuthRepository authRepository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            IConfiguration configuration,
            EmailService emailService,
            JwtHelperService jwtHelperService,
            IUserTrackingService userTrackingService,
            RedisService redisService,
            IWebHostEnvironment env,
            IDisposableEmailChecker disposableEmailChecker
            )
        {
            _authRepository = authRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _configuration = configuration;
            _emailService = emailService;
            _jwtHelperService = jwtHelperService;
            _userTrackingService = userTrackingService;
            _redisService = redisService;
            _env = env;
            _emailChecker = disposableEmailChecker;
        }

        public async Task<Result<LoginResponse>> Login(LoginDto loginDto)
        {
            #region API Validations
            if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
                return Result<LoginResponse>.BadRequest("Please provide username and passowrd, Bad request");
            #endregion

            LoginResponse? loginResponse = new();

            User? result = await _authRepository.Login(loginDto);

            if (result == null)
                return Result<LoginResponse>.NotFound("Provided username not found");

            var userDetail = await _userService.GetUserByEmail(loginDto.Email);

            if (Helper.VerifyPassword(loginDto.Password, userDetail.Data?.PasswordHash ?? ""))
            {
                loginResponse.Id = result.Id;
                loginResponse.Name = result.FirstName + " " + result.LastName;
                loginResponse.Email = result.Email;
                loginResponse.IsUserVerified = result.IsUserVerified;
                loginResponse.RoleId = result.RoleId;
                loginResponse.IsActive = result.IsActive;
                loginResponse.AccessToken = _jwtHelperService.GenerateJwtToken(result.Email, result.RoleId.ToString());
                loginResponse.RefreshToken = _jwtHelperService.GenerateRefreshToken(
                    result.Email, 
                    TimeSpan.FromMinutes(_configuration.GetValue<int>("Jwt:RefreshExpiresInMinutes")), 
                    _configuration.GetValue<string>("Jwt:RefreshKey") ?? "",
                    result.RoleId.ToString());
                SetCookies(loginResponse.AccessToken, loginResponse.RefreshToken);
                await _redisService.SetValueAsync(loginResponse.Id.ToString(), loginResponse.AccessToken);
            }
            else
            {
                return Result<LoginResponse>.BadRequest("Invalid password please try again");
            }

            await _redisService.SetValueAsync($"UserId_ ${loginResponse.Id}", $"_AccessToken_${ loginResponse.AccessToken}_RefreshToken_${ loginResponse.RefreshToken}");
            await SaveUserTrackingDetails(loginResponse);

            return Result<LoginResponse>.Success(loginResponse);
        }

        public async Task<Result<RegisterResponse>> Register(RegisterDto registerDto)
        {
            #region API Validations
            if (registerDto.FirstName.Length < 2)
                return Result<RegisterResponse>.BadRequest("Please provide firstName at least 2 character");
            if (registerDto.LastName.Length < 2)
                return Result<RegisterResponse>.BadRequest("Please provide lastName at least 2 character");
            if (string.IsNullOrWhiteSpace(registerDto.Email))
                return Result<RegisterResponse>.BadRequest("Please provide valid email address");
            if(registerDto.Email.Length > 200)
                return Result<RegisterResponse>.BadRequest("Email should not more then 200 character");
            if (string.IsNullOrWhiteSpace(registerDto.Password) || string.IsNullOrWhiteSpace(registerDto.ConfirmPassword))
                return Result<RegisterResponse>.BadRequest("Please provide valid password");
            if(registerDto.Password != registerDto.ConfirmPassword)
                return Result<RegisterResponse>.BadRequest("Password should match");
            if(!registerDto.Email.IsEmailValid())
                return Result<RegisterResponse>.BadRequest("Please provide valid email address");
            if (string.IsNullOrWhiteSpace(registerDto.FirstName))
                return Result<RegisterResponse>.BadRequest("Please provide firstname");
            if (string.IsNullOrWhiteSpace(registerDto.LastName))
                return Result<RegisterResponse>.BadRequest("Please provide lastname");
            if(registerDto.FirstName.Length > 200)
                return Result<RegisterResponse>.BadRequest("Firstname should not be more then 200 character");
            if (registerDto.FirstName.Length > 200)
                return Result<RegisterResponse>.BadRequest("Firstname should not be more then 200 character");
            if (await IsEmailAddressExists(registerDto.Email))
                return Result<RegisterResponse>.BadRequest("Email already exists");
            if(!Helper.IsPasswordFall(registerDto.Password, registerDto.FirstName, registerDto.LastName))
                return Result<RegisterResponse>.BadRequest("Provide at least one upper case and one lower case and one numeric and one special character and passoword should not start with your name.");
            if (registerDto.Password.Length > 512 || registerDto.ConfirmPassword.Length > 512)
                return Result<RegisterResponse>.BadRequest("Password should not be more then 512 character");
            if(await _emailChecker.IsDisposableEmail(registerDto.Email))
                return Result<RegisterResponse>.BadRequest("Provided email domain not supported please try different");
            #endregion

            var generateOtp = Helper.GenerateOTP();

            User user = new()
            {
                Id = registerDto.Id,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Name = registerDto.FirstName + " " + registerDto.LastName,
                PasswordHash = Helper.HashPassword(registerDto.Password),
                IsUserVerified = false,
                IsActive = false,
                OTP = generateOtp,
                OTPExpire = DateTime.UtcNow.AddMinutes(_configuration.GetValue<double>("OTP:Expiration")),
                RoleId = (int)UserRole.Standard,
                Created = DateTime.Now,
                IsTeamsAndConditionAccepted = registerDto.IsTeamsAndConditionAccepted,
            };

            var savedUser = await _userService.SaveUser(user);

            //await _emailService.SendOtpAsync(registerDto.Email, generateOtp);
            
            var data = _mapper.Map<RegisterResponse>(savedUser.Data);

            if(data.IsActive && data.IsUserVerified && !data.IsDeleted && data.IsTeamsAndConditionAccepted)
            {
                data.AccessToken = _jwtHelperService.GenerateJwtToken(data.Email, data.RoleId.ToString());
                data.RefreshToken = _jwtHelperService.GenerateRefreshToken(
                    data.Email, 
                    TimeSpan.FromMinutes(_configuration.GetValue<int>("Jwt:RefreshExpiresInMinutes")), 
                    _configuration.GetValue<string>("Jwt:RefreshKey") ?? "",
                    data.RoleId.ToString());
                await _redisService.SetValueAsync($"UserId_ ${data.Id}", $"_AccessToken_${data.AccessToken}_RefreshToken_${data.RefreshToken}");
            }

            return Result<RegisterResponse>.Success(data);
        }

        public async Task<Result<SendOTPResponse>> SendOTP(SendOtpDto sendOtpDto)
        {
            #region API Validations
            if (string.IsNullOrWhiteSpace(sendOtpDto.OTP))
                return Result<SendOTPResponse>.BadRequest("Please provide OTP, Bad Request");
            if(sendOtpDto.UserId < 0)
                return Result<SendOTPResponse>.BadRequest("Please provide UserId, Bad Request");
            #endregion

            var userDetail = await _userService.GetUserById(sendOtpDto.UserId);
            SendOTPResponse res = new();

            if(userDetail != null && userDetail.Data?.OTP != null)
            {
                if(sendOtpDto.OTP == userDetail.Data.OTP && !Helper.IsOtpExpired(userDetail.Data.OTPExpire))
                {
                    res.IsOtpSend = true;
                    var token = _jwtHelperService.GenerateJwtToken(userDetail.Data.Email, userDetail.Data.RoleId.ToString());
                    var refreshToken = _jwtHelperService.GenerateRefreshToken(
                        userDetail.Data.Email, 
                        TimeSpan.FromMinutes(_configuration.GetValue<int>("Jwt:RefreshExpiresInMinutes")), 
                        _configuration.GetValue<string>("Jwt:RefreshKey") ?? "",
                        userDetail.Data.RoleId.ToString());
                    res.AccessToken = token;
                    res.RefreshToken = refreshToken;
                    SetCookies(token, refreshToken);
                }
                else
                {
                    Result<SendOTPResponse>.Success(res, "Invalid otp either expired.");
                    RemoveAccessTokenCookie();
                }
            }

            return Result<SendOTPResponse>.Success(res);
        }

        public async Task<Result<EmailExitsResponse>> IsEmailExists(EmailExistsDto emailExistsDto)
        {
            #region API Validations
            if (string.IsNullOrWhiteSpace(emailExistsDto.Email))
                return Result<EmailExitsResponse>.BadRequest("Please provide email address");
            if(!emailExistsDto.Email.IsEmailValid())
                return Result<EmailExitsResponse>.BadRequest("Please provide valid email address");
            #endregion

            var emailExists = await IsEmailAddressExists(emailExistsDto.Email);
            EmailExitsResponse emailExitsResponse = new();

            if (emailExists)
                emailExitsResponse.IsEmailExists = true;

            return Result<EmailExitsResponse>.Success(emailExitsResponse);
        }

        public async Task<Result<RefreshTokenResponse>> RefreshToken(HttpRequest request, HttpResponse response)
        {
            var refreshToken = request.Cookies["RefreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Result<RefreshTokenResponse>.Unauthorized("Refresh token missing");

            var validated = _jwtHelperService.ValidateToken(refreshToken, _configuration.GetValue<string>("Jwt:Key"));
            if (validated == null)
                return Result<RefreshTokenResponse>.Unauthorized("Invalid token");

            var email = validated.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var roleId = validated.Claims.FirstOrDefault(r => r.Type == ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(email))
                return Result<RefreshTokenResponse>.Unauthorized("Invalid token payload");

            var user = await _userService.GetUserByEmail(email);
            if (user.Data == null)
                return Result<RefreshTokenResponse>.NotFound("User not found");

            var accessToken = _jwtHelperService.GenerateJwtToken(email, roleId);
            var newRefreshToken = _jwtHelperService.GenerateRefreshToken(
                email,
                TimeSpan.FromMinutes(_configuration.GetValue<int>("Jwt:RefreshExpiresInMinutes")),
                _configuration["Jwt:RefreshKey"] ?? "", roleId);

            SetCookies(accessToken, newRefreshToken);

            var res = new RefreshTokenResponse()
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            };

            return Result<RefreshTokenResponse>.Success(res);
        }

        public async Task<Result<ForgotPasswordResponse>> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            #region API Validations
            if (string.IsNullOrWhiteSpace(forgotPasswordDto.Email))
                return Result<ForgotPasswordResponse>.BadRequest("Please provide email");
            if (!await IsEmailAddressExists(forgotPasswordDto.Email))
                return Result<ForgotPasswordResponse>.BadRequest("Provided email not exists");
            if(string.IsNullOrWhiteSpace(forgotPasswordDto.Password))
                return Result<ForgotPasswordResponse>.BadRequest("Please provide password");
            if (string.IsNullOrWhiteSpace(forgotPasswordDto.ConfirmPassword))
                return Result<ForgotPasswordResponse>.BadRequest("Please provide ConfirmPassword");
            if (forgotPasswordDto.Password != forgotPasswordDto.ConfirmPassword)
                return Result<ForgotPasswordResponse>.BadRequest("Password should be match");
            #endregion

            var userDetail = await _userService.GetUserByEmail(forgotPasswordDto.Email);

            if(Helper.VerifyPassword(forgotPasswordDto.Password, userDetail.Data.PasswordHash))
                return Result<ForgotPasswordResponse>.BadRequest("Password should not be previous");

            userDetail.Data.PasswordHash = Helper.HashPassword(forgotPasswordDto.Password);
            var user = _mapper.Map<User>(userDetail.Data);

            var updatedUser = await _userService.SaveUser(user);
            ForgotPasswordResponse forgotPasswordResponse = new()
            {
                Id = updatedUser.Data.Id,
                Email = updatedUser.Data.Email,
                AccessToken = _jwtHelperService.GenerateJwtToken(updatedUser.Data.Email, updatedUser.Data.RoleId.ToString())
            };

            string forwardedIp = _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"];
            string realIp = _httpContextAccessor.HttpContext.Request.Headers["X-Real-IP"];
            string userIp = !string.IsNullOrEmpty(forwardedIp) ? forwardedIp.Split(',')[0] : realIp ?? _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            string userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"];

            SendPasswordChangeEmailDto sendPasswordChangeEmailDto = new()
            {
                Email = updatedUser.Data.Email,
                ForwardedIp = forwardedIp,
                RealIp = realIp,
                UserIp = userIp,
                UserAgent = userAgent
            };

            //await _emailService.SendPasswordChangeEmail(sendPasswordChangeEmailDto);

            return Result<ForgotPasswordResponse>.Success(forgotPasswordResponse);
        }

        public async Task<Result<RegisterWithOAuthResponse>> RegisterWithOAuth(RegisterWithOAuthDto registerWithOAuthDto)
        {
            #region API Validations
            if (string.IsNullOrWhiteSpace(registerWithOAuthDto.Provider))
                return Result<RegisterWithOAuthResponse>.BadRequest("Please provide sing up with value");
            #endregion

            var redirectUrl = $"/api/auth/callback?provider={registerWithOAuthDto.Provider}";
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            var provider = registerWithOAuthDto.Provider?.ToLowerInvariant();

            var item = provider switch
            {
                "google" => new ChallengeResult(GoogleDefaults.AuthenticationScheme, properties),
                "facebook" => new ChallengeResult(FacebookDefaults.AuthenticationScheme, properties),
                "twitter" => new ChallengeResult(TwitterDefaults.AuthenticationScheme, properties),
                _ => new BadRequestObjectResult("Unsupported provider") as IActionResult
            };

            var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();

            if (!authenticateResult.Succeeded)
                return Result<RegisterWithOAuthResponse>.BadRequest("Authentication failed");

            var claims = authenticateResult.Principal?.Identities.FirstOrDefault()?.Claims
                .Select(c => new { c.Type, c.Value });

            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("AccessToken");

            RegisterWithOAuthResponse registerWithOAuthResponse = new()
            {
                AccessToken = accessToken,
                User = (IEnumerable<System.Security.Claims.Claim>)claims,
                Provider = item.ToString()
            };

            return Result<RegisterWithOAuthResponse>.Success(registerWithOAuthResponse);
        }

        public async Task<Result<LogoutResponse>> Logout(HttpRequest request, HttpResponse httpResponse)
        {
            return await Task.Run(() =>
            {
                if (httpResponse == null)
                    return Result<LogoutResponse>.BadRequest("HttpResponse is unavailable.");
                if (request == null)
                    return Result<LogoutResponse>.BadRequest("HttpRequest is unavailable.");

                httpResponse.Cookies.Delete("AccessToken", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, 
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddDays(-1) 
                });

                httpResponse.Cookies.Delete("RefreshToken", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddDays(-1)
                });

                LogoutResponse logoutResponse = new()
                {
                    IsLogout = true
                };

                return Result<LogoutResponse>.Success(logoutResponse);
            });
        }

        public async Task<Result<IsEmailDomainSupportResponse>> IsEmailDomainSupport(IsEmailDomainSupportDto obj)
        {
            #region API VALIDATIONS
            if (string.IsNullOrEmpty(obj.Email))
                return Result<IsEmailDomainSupportResponse>.BadRequest("Please provide email address");
            #endregion

            IsEmailDomainSupportResponse response = new();

            if (await _emailChecker.IsDisposableEmail(obj.Email))
            {
                response.IsEmailSupport = false;
                return Result<IsEmailDomainSupportResponse>.Success(response);
            }

            response = new() { IsEmailSupport = true };

            return Result<IsEmailDomainSupportResponse>.Success(response);
        }


        #region Private Methods
        private void SetCookies(string token, string refreshToken)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = _env.IsDevelopment() ? false : true,
                SameSite = _env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpiresInMinutes")),
                Path = "/"
            };

            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = _env.IsDevelopment() ? false : true,
                SameSite = _env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:RefreshExpiresInMinutes")),
                Path = "/"
            };

            _httpContextAccessor.HttpContext?.Response.Cookies.Append("AccessToken", token, options);
            _httpContextAccessor.HttpContext?.Response.Cookies.Append("RefreshToken", refreshToken, refreshCookieOptions);
        }

        private void RemoveAccessTokenCookie()
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete("AccessToken");
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete("RefreshToken");
        }

        private async Task<bool> IsEmailAddressExists(string email)
        {
            var userDetail = await _userService.GetUserByEmailId(email);
            return (bool)userDetail.Data?.IsEmailExists;
        }

        private async Task SaveUserTrackingDetails(LoginResponse loginResponse)
        {
            var data = await _userTrackingService.GetUserTrackingById(loginResponse.Id);
            var userTrackingDetails = _userTrackingService.GetUserTrackingDetailFromIp(_httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString());

            UserTrackingDto userTrackingDto = new()
            {
                Id = 0,
                UserId = loginResponse.Id,
                IpAddress = userTrackingDetails.Result.IpAddress,
                Latitude = userTrackingDetails.Result.Latitude,
                Longitude = userTrackingDetails.Result.Longitude,
                LoginTime = DateTime.UtcNow,
                Country = userTrackingDetails.Result.Country,
                City = userTrackingDetails.Result.City,
                UserAgent = userTrackingDetails.Result.UserAgent
            };

            if(data.Data != null && data.Data.IpAddress != null && data.Data.IpAddress != userTrackingDto.IpAddress)
            {
                await _userTrackingService.SaveUserTracking(userTrackingDto);
            }
        }
        #endregion
    }
}
