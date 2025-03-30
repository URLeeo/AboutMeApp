using AboutMeApp.Application.Dtos.Template;
using AboutMeApp.Application.Dtos.UserProfile;
using AboutMeApp.Common.Shared;

namespace AboutMeApp.Application.Abstractions.Services;

public interface IUserProfileService
{
    Task<BaseResponse<UserProfileGetDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<Pagination<UserProfileGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<BaseResponse<Pagination<UserProfileGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<BaseResponse<UserProfileCreateDto>> CreateAsync(UserProfileCreateDto userProfileCreateDto);
    Task<BaseResponse<UserProfileUpdateDto>> UpdateAsync(Guid id, UserProfileUpdateDto userProfileUpdateDto);
    Task<BaseResponse<object>> DeleteAsync(Guid id);
}
