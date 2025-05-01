using AutoMapper;
using likhitan.Common.Services;
using likhitan.Entities;
using likhitan.Repository;
using likhitan_api.Models;
using likhitan_api.Models.ClientDto;

namespace likhitan.Services
{
    public interface IAuthorService
    {
        Task<Result<BecomeAuthorResponse>> BecomeAuthor(BecomeAuthorDto becomeAuthorDto);
        Task<Result<AuthorResponse>> GetAuthorById(int id);
    }
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly JwtHelperService _jwtHelperService;

        public AuthorService(
            IAuthorRepository authorRepository, 
            IMapper mapper, 
            IUserRepository userRepository,
            JwtHelperService jwtHelperService) 
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _jwtHelperService = jwtHelperService;
        }

        public async Task<Result<BecomeAuthorResponse>> BecomeAuthor(BecomeAuthorDto becomeAuthorDto)
        {
            #region API VALIDATIONS
            if (becomeAuthorDto?.DOB == null)
                return Result<BecomeAuthorResponse>.BadRequest("Please provide date of birth");
            if(becomeAuthorDto.Gender <= 0)
                return Result<BecomeAuthorResponse>.BadRequest("Please provide gender");
            #endregion

            var loggedInUerDetail = _jwtHelperService.GetLoggedInUserDetails();

            if(string.IsNullOrEmpty(loggedInUerDetail.Email))
            {
                return Result<BecomeAuthorResponse>.InternalServerError("cannot able to fetch user emailId please try again");
            }

            var userDetail = await _userRepository.GetUserDetailByEmailId(loggedInUerDetail.Email);

            var author = new Author()
            {
                Id = 0,
                Name = userDetail.Name,
                UserId = userDetail.Id,
                IsActive = true,
                Created = DateTime.Now,
                IsDeleted = false
            };
            await _authorRepository.SaveBecomeAuthor(author);

            var authorDetail = await GetAuthorById(author.Id);

            var authorResult = new BecomeAuthorResponse()
            {
                AuthorId = authorDetail.Data.Id
            };
            return Result<BecomeAuthorResponse>.Success(authorResult);
        }

        public async Task<Result<AuthorResponse>> GetAuthorById(int id)
        {
            if(id == 0)
            {
                return Result<AuthorResponse>.BadRequest("Please provide author id");
            }
            var authorDetail = await _authorRepository.GetAuthorById(id);
            var mappedData = _mapper.Map<Author, AuthorResponse>(authorDetail);
            return Result<AuthorResponse>.Success(mappedData);
        }
    }
}
