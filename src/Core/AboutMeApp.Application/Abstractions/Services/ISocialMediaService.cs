using AboutMeApp.Application.Dtos.Experience;
using AboutMeApp.Application.Dtos.SocialMedia;
using AboutMeApp.Common.Shared;

namespace AboutMeApp.Application.Abstractions.Services;

public interface ISocialMediaService
{
    Task<BaseResponse<SocialMediaGetDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<Pagination<SocialMediaGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<BaseResponse<Pagination<SocialMediaGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<BaseResponse<SocialMediaCreateDto>> CreateAsync(SocialMediaCreateDto socialmediaCreateDto);
    Task<BaseResponse<SocialMediaUpdateDto>> UpdateAsync(Guid id, SocialMediaUpdateDto socialmediaUpdateDto);
    Task<BaseResponse<object>> DeleteAsync(Guid id);
}
