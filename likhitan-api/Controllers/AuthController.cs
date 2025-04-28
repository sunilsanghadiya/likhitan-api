using likhitan.Common.Services;
using likhitan.Models;
using likhitan.Models.ClientDto;
using likhitan.Services;
using likhitan_api.Models;
using likhitan_api.Models.ClientDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace likhitan.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        public IAuthService _authService;
        public AuthController(IAuthService authService) 
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<Result<LoginResponse>> Login([FromBody] LoginDto loginDto) =>
            await _authService.Login(loginDto);

        [HttpPost("Register")]
        public async Task<Result<RegisterResponse>> Register([FromBody] RegisterDto registerDto) =>
            await _authService.Register(registerDto);

        [HttpPost("SendOTP")]
        public async Task<Result<SendOTPResponse>> SendOTP([FromBody] SendOtpDto sendOtpDto) =>
            await _authService.SendOTP(sendOtpDto);

        [HttpPost("IsEmailExists")]
        public async Task<Result<EmailExitsResponse>> IsEmailExists(EmailExistsDto emailExistsDto) =>
            await _authService.IsEmailExists(emailExistsDto);

        [HttpPost("RefreshToken")]
        public async Task<Result<RefreshTokenResponse>> RefreshToken() =>
            await _authService.RefreshToken(Request, Response);

        [HttpPost("ForgotPassword")]
        public async Task<Result<ForgotPasswordResponse>> ForgotPassword(ForgotPasswordDto forgotPasswordDto) =>
            await _authService.ForgotPassword(forgotPasswordDto);

        [HttpPost("RegisterWithOAuth")]
        public async Task<Result<RegisterWithOAuthResponse>> RegisterWithOAuth(RegisterWithOAuthDto registerWithOAuthDto) =>
            await _authService.RegisterWithOAuth(registerWithOAuthDto);

        [HttpGet("Logout")]
        public async Task<Result<LogoutResponse>> Logout() =>
            await _authService.Logout(Request, Response);

        [HttpPost("IsEmailDomainSupport")]
        public async Task<Result<IsEmailDomainSupportResponse>> IsEmailDomainSupport(IsEmailDomainSupportDto obj) =>
            await _authService.IsEmailDomainSupport(obj);

    }
}
