using AboutMeApp.Application.Dtos.SocialMedia;
using AboutMeApp.Application.Dtos.Template;
using AboutMeApp.Common.Shared;
using AboutMeApp.Domain.Entities;
using AutoMapper;

namespace AboutMeApp.Application.Profiles;

public class TemplateProfile : Profile
{
    public TemplateProfile()
    {
        CreateMap<Template, TemplateGetDto>().ReverseMap();
        CreateMap<Template, TemplateCreateDto>().ReverseMap();
        CreateMap<Template, TemplateUpdateDto>().ReverseMap();
        CreateMap(typeof(Pagination<>), typeof(Pagination<>));
    }
}
