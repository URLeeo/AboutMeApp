using AboutMeApp.Application.Dtos.Template;
using AboutMeApp.Application.Dtos.UserProfile;
using AboutMeApp.Common.Shared;
using AboutMeApp.Domain.Entities;
using AutoMapper;

namespace AboutMeApp.Application.Profiles;

public class UserProfileProfile : Profile
{
    public UserProfileProfile()
    {
        CreateMap<UserProfile, UserProfileGetDto>().ReverseMap();
        CreateMap<UserProfile, UserProfileCreateDto>().ReverseMap();
        CreateMap<UserProfile, UserProfileUpdateDto>().ReverseMap();
        CreateMap(typeof(Pagination<>), typeof(Pagination<>));
    }
}
