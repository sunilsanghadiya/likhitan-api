using likhitan.Common.Services;
using likhitan.Models;
using likhitan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace likhitan.Controllers
{
    [Route("api/profile")]
    [Authorize]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        public IProfileService _profileService;
        public ProfileController(IProfileService profileService) 
        {
            _profileService = profileService;
        }

        [HttpGet("GetUserProfileByUserId")]
        public async Task<Result<ProfileResponse>> GetUserProfileByUserId(int id) =>
            await _profileService.GetUserProfileByUserId(id);
    }
}
