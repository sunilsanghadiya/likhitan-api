using AutoMapper;
using likhitan.Common.Services;
using likhitan.Entities;
using likhitan.Models;
using likhitan.Repository;

namespace likhitan.Services
{
    public interface IUserService
    {
        Task<Result<User>> SaveUser(User user);
        Task<Result<UserResponse>> GetUserByEmail(string email);
        Task<Result<UserResponse>> GetUserById(int id);
        Task<Result<UserEmailExistsResponse>> GetUserByEmailId(string email);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public IMapper _mapper;
        public RedisService _redisService;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            RedisService redisService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<Result<User>> SaveUser(User user)
        {
            User newUser = await _userRepository.SaveUser(user);
            //var data = _mapper.Map<Result<User>>(user);
            return Result<User>.Success(newUser); 
        }

        public async Task<Result<UserResponse>> GetUserByEmail(string email)
        {
            #region API Validation
            if (string.IsNullOrEmpty(email))
                Result<User>.BadRequest("Please provide email address");
            #endregion

            var user = await _userRepository.GetUserByEmail(email);

            var data = _mapper.Map<UserResponse>(user);
            return Result<UserResponse>.Success(data);
        }

        public async Task<Result<UserEmailExistsResponse>> GetUserByEmailId(string email)
        {
            #region API Validation
            if (string.IsNullOrEmpty(email))
                Result<User>.BadRequest("Please provide email address");
            #endregion

            UserEmailExistsResponse userEmailExistsResponse = new();

            var data = await _redisService.GetValueAsync(email);

            if(data != null)
            {
                if (data != null && data.Any())
                {
                    userEmailExistsResponse = new()
                    {
                        IsEmailExists = true
                    };
                }
            }
            else
            {
                var user = await _userRepository.GetUserByEmailId(email);
                //await _redisService.SetValueAsync(email, email, TimeSpan.FromMinutes(1));

                if (user != null && user.Any())
                {
                    userEmailExistsResponse = new()
                    {
                        IsEmailExists = true
                    };
                }
            }
           
            return Result<UserEmailExistsResponse>.Success(userEmailExistsResponse);
        }

        public async Task<Result<UserResponse>> GetUserById(int id)
        {
            #region API Validation
            if (id < 0)
                Result<User>.BadRequest("Please provide id");
            #endregion

            var user = await _userRepository.GetSimpleUserById(id);
            var data = _mapper.Map<UserResponse>(user);
            return Result<UserResponse>.Success(data);
        }
    }
}
