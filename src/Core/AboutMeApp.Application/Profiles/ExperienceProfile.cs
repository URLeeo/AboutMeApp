using AboutMeApp.Application.Dtos.Experience;
using AboutMeApp.Common.Shared;
using AboutMeApp.Domain.Entities;
using AutoMapper;

namespace AboutMeApp.Application.Profiles;

public class ExperienceProfile : Profile
{
    public ExperienceProfile()
    {
        CreateMap<Experience, ExperienceGetDto>().ReverseMap();
        CreateMap<Experience, ExperienceCreateDto>().ReverseMap();
        CreateMap<Experience, ExperienceUpdateDto>().ReverseMap();
        CreateMap(typeof(Pagination<>), typeof(Pagination<>));
    }
}
