using likhitan.Common.Services;
using likhitan_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace likhitan.Controllers
{
    [Route("api/validate")]
    [ApiController]
    [Authorize]
    public class ValidateController : ControllerBase
    {
        public IConfiguration Configuration { get; set; }

        public ValidateController(IConfiguration configuration) 
        {
            Configuration = configuration;
        }

        [HttpGet("checkAuth")]
        public Result<CheckAuthResponse> CheckAuth()
        {
            CheckAuthResponse response = new()
            {
                IsAuthenticated = true,
            };
            return Result<CheckAuthResponse>.Success(response);
        }

    }
}
