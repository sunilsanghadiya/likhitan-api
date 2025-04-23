using AutoMapper;
using likhitan.Common.Services;
using likhitan.Models;

namespace likhitan.Services
{
    public interface IProfileService
    {
        Task<Result<ProfileResponse>> GetUserProfileByUserId(int id);
    }
    public class ProfileService : IProfileService
    {
        private IUserService _userService;
        public IMapper _mapper;
        public ProfileService(IUserService userService, IMapper mapper) 
        {
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<Result<ProfileResponse>> GetUserProfileByUserId(int id)
        {
            #region API Validations
            if (id == 0)
                return Result<ProfileResponse>.BadRequest("Please provide valid user id");
            #endregion
            var userProfileDetails = await _userService.GetUserById(id);

            var data = _mapper.Map<ProfileResponse>(userProfileDetails.Data);
            return Result<ProfileResponse>.Success(data);
        }
    }
}
