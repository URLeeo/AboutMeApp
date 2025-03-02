using AboutMeApp.Application.Dtos.Certificate;
using AboutMeApp.Common.Shared;

namespace AboutMeApp.Application.Abstractions.Services;

public interface ICertificateService
{
    Task<BaseResponse<CertificateGetDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<Pagination<CertificateGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<BaseResponse<Pagination<CertificateGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<BaseResponse<CertificateCreateDto>> CreateAsync(CertificateCreateDto certificateCreateDto);
    Task<BaseResponse<CertificateUpdateDto>> UpdateAsync(Guid id, CertificateUpdateDto certificateUpdateDto);
    Task<BaseResponse<object>> DeleteAsync(Guid id);
}

