using AboutMeApp.Application.Abstractions.Services;
using AboutMeApp.Application.Dtos.Identity;
using AboutMeApp.Application.Validations.Identity;
using AboutMeApp.Common.Shared;
using AboutMeApp.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Security.Claims;

namespace AboutMeApp.Persistence.Implementations.Services;

public class AuthService : IAuthService
{
    private IJwtService _jwtService { get; }
    private UserManager<User> _userManager { get; }
    private IMapper _mapper { get; }
    private IConfiguration _configuration { get; }

    public AuthService(IJwtService jwtService, UserManager<User> userManager, IMapper mapper, IConfiguration configuration)
    {
        _jwtService = jwtService;
        _userManager = userManager;
        _mapper = mapper;
        _configuration = configuration;
    }
    public async Task<BaseResponse<object>> LoginAsync(LoginDto loginDto)
    {
        var validator = new LoginDtoValidator();
        var validationResult = await validator.ValidateAsync(loginDto);
        if (!validationResult.IsValid)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = string.Join(";", validationResult.Errors.Select(e => e.ErrorMessage)),
                Data = null
            };
        }
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Invalid email or password."
            };
        }
        var claims = new List<Claim>();
        claims.AddRange(
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Surname, user.Surname ?? ""),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
        ]);
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        string token = _jwtService.GenerateAccessToken(claims);
        _ = int.TryParse(_configuration["JwtSettings:AccessTokenExpirationHours"], out int accessTokenExpiryTime);
        string refreshToken = _jwtService.GenerateRefreshToken();
        _ = int.TryParse(_configuration["JwtSettings:RefreshTokenExpirationHours"], out int refreshTokenExpiryTime);
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(refreshTokenExpiryTime);
        await _userManager.UpdateAsync(user);
        return new BaseResponse<object>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "User successfully logged in",
            Data = new
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiryTime,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = refreshTokenExpiryTime
            }
        };
    }

    public async Task<BaseResponse<object>> RefreshToken(string refreshtoken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshtoken);
        if (user is null)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "This refreshToken is not valid",
                Data = null
            };
        }
        if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "This refreshToken is expired",
                Data = null
            };
        }
        var claims = new List<Claim>();
        claims.AddRange(
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Surname, user.Surname ?? ""),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
        ]);
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        string token = _jwtService.GenerateAccessToken(claims);
        _ = int.TryParse(_configuration["JwtSettings:AccessTokenExpirationHours"], out int accessTokenExpiryTime);
        string refreshToken = _jwtService.GenerateRefreshToken();
        _ = int.TryParse(_configuration["JwtSettings:RefreshTokenExpirationHours"], out int refreshTokenExpiryTime);
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(refreshTokenExpiryTime);
        await _userManager.UpdateAsync(user);
        return new BaseResponse<object>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "New tokens are created",
            Data = new
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiryTime,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = refreshTokenExpiryTime
            }
        };

    }
    public async Task<BaseResponse<object>> RegisterAsync(RegisterDto registerDto)
    {
        var validator = new RegisterDtoValidator();
        var validationResult = await validator.ValidateAsync(registerDto);
        if (!validationResult.IsValid)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = string.Join(";", validationResult.Errors.Select(e => e.ErrorMessage))
            };
        }
        var registeredUser = await _userManager.FindByEmailAsync(registerDto.Email);
        if (registeredUser is not null)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "This email already registered."
            };
        }
        User user = _mapper.Map<User>(registerDto);
        user.UserName = registerDto.Email;
        user.SecurityStamp = Guid.NewGuid().ToString().Replace("-", "");
        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = string.Join("; ", result.Errors.Select(e => e.Description))
            };
        }
        await _userManager.AddToRoleAsync(user, "user");

        return new BaseResponse<object>
        {
            Message = "User registered successfully.",
            StatusCode = HttpStatusCode.Created
        };
    }
}
