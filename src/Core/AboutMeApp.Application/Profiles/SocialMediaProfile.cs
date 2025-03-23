using AboutMeApp.Application.Dtos.Experience;
using AboutMeApp.Application.Dtos.SocialMedia;
using AboutMeApp.Common.Shared;
using AboutMeApp.Domain.Entities;
using AutoMapper;

namespace AboutMeApp.Application.Profiles;

public class SocialMediaProfile : Profile
{
    public SocialMediaProfile()
    {
        CreateMap<SocialMedia, SocialMediaGetDto>().ReverseMap();
        CreateMap<SocialMedia, SocialMediaCreateDto>().ReverseMap();
        CreateMap<SocialMedia, SocialMediaUpdateDto>().ReverseMap();
        CreateMap(typeof(Pagination<>), typeof(Pagination<>));
    }
}
