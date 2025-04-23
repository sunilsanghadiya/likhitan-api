using AutoMapper;
using likhitan.Common.Services;
using likhitan.Models;
using likhitan.Repository;

namespace likhitan.Services
{
    public interface IUserRoleService
    {
        Task<Result<UserRoleResponse>> GetUserRoleById(int id);
    }
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private IMapper _mapper;
        public UserRoleService(IUserRoleRepository userRoleRepository, IMapper mapper) 
        { 
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
        }

        public async Task<Result<UserRoleResponse>> GetUserRoleById(int id)
        {
            var userRole = await _userRoleRepository.GetUserRoleById(id);
            var data = _mapper.Map<Result<UserRoleResponse>>(userRole);
            return data;
        }
    }
}
