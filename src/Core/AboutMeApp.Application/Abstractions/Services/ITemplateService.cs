using AboutMeApp.Application.Dtos.SocialMedia;
using AboutMeApp.Application.Dtos.Template;
using AboutMeApp.Common.Shared;

namespace AboutMeApp.Application.Abstractions.Services;

public interface ITemplateService
{
    Task<BaseResponse<TemplateGetDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<Pagination<TemplateGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<BaseResponse<Pagination<TemplateGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<BaseResponse<TemplateCreateDto>> CreateAsync(TemplateCreateDto templateCreateDto);
    Task<BaseResponse<TemplateUpdateDto>> UpdateAsync(Guid id, TemplateUpdateDto templateUpdateDto);
    Task<BaseResponse<object>> DeleteAsync(Guid id);
}
