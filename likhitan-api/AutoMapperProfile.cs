using AutoMapper;
using likhitan.Entities;
using likhitan.Models;
using likhitan.Models.ClientDto;

namespace likhitan
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<User, LoginResponse>();
            CreateMap<User, UserResponse>();
            CreateMap<User, RegisterResponse>();
            CreateMap<UserResponse, ProfileResponse>();
            CreateMap<UserTracking, UserTrackingResponse>();
            CreateMap<UserTrackingDto, UserTracking>();
            CreateMap<UserResponse, User>();
        }
    }
}
