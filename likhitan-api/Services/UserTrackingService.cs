using System.Text.Json;
using AutoMapper;
using likhitan.Common.Services;
using likhitan.Entities;
using likhitan.Models;
using likhitan.Models.ClientDto;
using likhitan.Repository;

namespace likhitan.Services
{
    public interface IUserTrackingService
    {
        Task<Result<UserTrackingResponse>> GetUserTrackingById(int id);
        Task<Result<UserTrackingResponse>> SaveUserTracking(UserTrackingDto userTrackingDto);
        Task<InternalUserTrackingResponse> GetUserTrackingDetailFromIp(string ipAddress);
    }
    public class UserTrackingService : IUserTrackingService
    {
        private IUserTrackingRepository _userTrackingRepository;
        public IMapper _mapper;
        private readonly string _geoApiUrl = "http://ip-api.com/json/";
        public HttpClient _httpClient;

        public UserTrackingService(
            IUserTrackingRepository userTracking, 
            IMapper mapper, 
            HttpClient httpClient
            ) 
        {
            _userTrackingRepository = userTracking;
            _mapper = mapper;
            _httpClient = httpClient;
        }

        public async Task<Result<UserTrackingResponse>> GetUserTrackingById(int userId)
        {
            #region API Validation
            if (userId < 0)
                return Result<UserTrackingResponse>.BadRequest("Please provide valid id");
            #endregion

            var response = await _userTrackingRepository.GetUserTrackingById(userId);
            var data = _mapper.Map<UserTrackingResponse>(response);

            return Result<UserTrackingResponse>.Success(data);
        }

        public async Task<Result<UserTrackingResponse>> SaveUserTracking(UserTrackingDto userTrackingDto)
        {
            #region API Validations
            if (userTrackingDto.UserId < 0)
                return Result<UserTrackingResponse>.BadRequest("Please provide userId");
            #endregion

            var prepareEntityDto = _mapper.Map<UserTracking>(userTrackingDto);
            var response = await _userTrackingRepository.SaveUserTracking(prepareEntityDto);
            var data = _mapper.Map<UserTrackingResponse>(response);

            return Result<UserTrackingResponse>.Success(data);
        }

        public async Task<InternalUserTrackingResponse> GetUserTrackingDetailFromIp(string ipAddress)
        {
            var response = await _httpClient.GetStringAsync($"{_geoApiUrl}{ipAddress}");
            var locationData = System.Text.Json.JsonSerializer.Deserialize<InternalUserTrackingResponse>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return locationData;
        }
    }
}
