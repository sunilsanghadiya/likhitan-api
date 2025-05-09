using AutoMapper;
using likhitan.Entities;
using likhitan.Models;
using likhitan.Models.ClientDto;
using likhitan_api.Models;
using likhitan_api.Models.ClientDto;

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
            CreateMap<Author, AuthorResponse>();
            CreateMap<User, WholeUserResponse>();
            CreateMap<WholeUserResponse, User>();
            CreateMap<BlogCommentDto, BlogComments>();
        }
    }
}
