using likhitan.Common.Services;
using likhitan.Services;
using likhitan_api.Models;
using likhitan_api.Models.ClientDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace likhitan.Controllers
{
    [Route("api/author")]
    [ApiController]
    [Authorize]
    public class AuthorController : ControllerBase
    {
        private IAuthorService _authorService;
        public AuthorController(IAuthorService authorService) 
        {
            _authorService = authorService;
        }

        [HttpGet("GetAuthorById")]
        public async Task<Result<AuthorResponse>> GetAuthorById(int id) =>
            await _authorService.GetAuthorById(id);

        [HttpPost("BecomeAuthor")]
        public async Task<Result<BecomeAuthorResponse>> BecomeAuthor(BecomeAuthorDto becomeAuthorDto) =>
            await _authorService.BecomeAuthor(becomeAuthorDto);

    }
}
