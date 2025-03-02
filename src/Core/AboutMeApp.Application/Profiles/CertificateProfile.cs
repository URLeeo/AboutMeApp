using AboutMeApp.Application.Dtos.Certificate;
using AboutMeApp.Domain.Entities;
using AutoMapper;

namespace AboutMeApp.Application.Profiles;

public class CertificateProfile : Profile
{
    public CertificateProfile()
    {
        CreateMap<Certificate, CertificateGetDto>()
            .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.ExpiryDate))
            .ReverseMap();
        CreateMap<Certificate, CertificateCreateDto>().ReverseMap();
        CreateMap<Certificate, CertificateUpdateDto>().ReverseMap();
    }
}
