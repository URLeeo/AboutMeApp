using AboutMeApp.Application.Dtos.Education;
using AboutMeApp.Common.Shared;
using AboutMeApp.Domain.Entities;
using AutoMapper;

namespace AboutMeApp.Application.Profiles;

public class EducationProfile : Profile
{
    public EducationProfile()
    {
        CreateMap<Education, EducationGetDto>().ReverseMap();
        CreateMap<Education, EducationCreateDto>().ReverseMap();
        CreateMap<Education, EducationUpdateDto>().ReverseMap();
        CreateMap(typeof(Pagination<>), typeof(Pagination<>));
    }
}
