using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace likhitan.Controllers
{
    [Route("api/validate")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class ValidateController : ControllerBase
    {
        public IConfiguration Configuration { get; set; }

        public ValidateController(IConfiguration configuration) 
        {
            Configuration = configuration;
        }

        [HttpGet("check-auth")]
        public IActionResult CheckAuth() => Ok(true);

    }
}
