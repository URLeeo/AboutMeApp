using AboutMeApp.Application.Dtos.Education;
using AboutMeApp.Application.Dtos.Experience;
using AboutMeApp.Common.Shared;

namespace AboutMeApp.Application.Abstractions.Services;

public interface IExperienceService
{
    Task<BaseResponse<ExperienceGetDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<Pagination<ExperienceGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<BaseResponse<Pagination<ExperienceGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<BaseResponse<ExperienceCreateDto>> CreateAsync(ExperienceCreateDto experienceCreateDto);
    Task<BaseResponse<ExperienceUpdateDto>> UpdateAsync(Guid id, ExperienceUpdateDto experienceUpdateDto);
    Task<BaseResponse<object>> DeleteAsync(Guid id);
}
