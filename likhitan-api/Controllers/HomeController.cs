using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace likhitan.Controllers
{
    [ApiController]
    [Route("")]
    [AllowAnonymous]
    public class HomeController : ControllerBase
    {
        public HomeController() { }

        [HttpGet("/")]
        public IActionResult Get()
        {
            return Ok("Welcome to likhitan api.");
        }
    }
}
