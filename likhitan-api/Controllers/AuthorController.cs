using likhitan.Services;
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

        //public async Task<Results<AuthorResponse>> GetAuthorById(int id) =>
        //    await _authorService.GetAuthorById(id);

        //public async Task<Results<BecomeAuthorResponse>>

    }
}
