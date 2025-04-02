using AboutMeApp.Application.Dtos.Identity;
using AboutMeApp.Domain.Entities;
using AutoMapper;

namespace AboutMeApp.Application.Profiles;

internal class IdentityProfile : Profile
{
    public IdentityProfile()
    {
        CreateMap<User, LoginDto>().ReverseMap();
        CreateMap<User, RegisterDto>().ReverseMap();
        CreateMap<User, VerifyUserDto>().ReverseMap();
    }
}
