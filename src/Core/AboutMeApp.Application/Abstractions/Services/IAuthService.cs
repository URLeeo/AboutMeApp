using AboutMeApp.Application.Dtos.Identity;
using AboutMeApp.Common.Shared;

namespace AboutMeApp.Application.Abstractions.Services;
public interface IAuthService
{
    Task<BaseResponse<object>> RegisterAsync(RegisterDto registerDto);
    Task<BaseResponse<object>> LoginAsync(LoginDto loginDto);
    Task<BaseResponse<object>> RefreshToken(string refreshtoken);
}

