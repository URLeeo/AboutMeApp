using AboutMeApp.Application.Dtos.Certificate;
using AboutMeApp.Application.Dtos.Education;
using AboutMeApp.Common.Shared;

namespace AboutMeApp.Application.Abstractions.Services;

public interface IEducationService
{
    Task<BaseResponse<EducationGetDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<Pagination<EducationGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<BaseResponse<Pagination<EducationGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<BaseResponse<EducationCreateDto>> CreateAsync(EducationCreateDto educationCreateDto);
    Task<BaseResponse<EducationUpdateDto>> UpdateAsync(Guid id, EducationUpdateDto educationUpdateDto);
    Task<BaseResponse<object>> DeleteAsync(Guid id);
}