using AboutMeApp.Application.Abstractions.Services;
using AboutMeApp.Common.Shared;
using AboutMeApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AboutMeApp.Persistence.Implementations.Services;

public class RoleService : IRoleService
{
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;

    public RoleService(RoleManager<Role> roleManager, UserManager<User> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }
    public async Task<BaseResponse<object>> CreateRoleAsync(string roleName)
    {
        if (await _roleManager.RoleExistsAsync(roleName))
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Role already exists."
            };
        }

        var role = new Role()
        {
            Name = roleName.ToLower(),
            NormalizedName = roleName.ToUpper(),
        };
        var result = await _roleManager.CreateAsync(role);

        if (result.Succeeded)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.Created,
                Message = "Role created successfully."
            };
        }

        return new BaseResponse<object>
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Message = "Failed to create role."
        };
    }
    public async Task<BaseResponse<bool>> RoleExistsAsync(string roleName)
    {
        var exists = await _roleManager.RoleExistsAsync(roleName.ToLower());
        return new BaseResponse<bool>
        {
            Data = exists,
            StatusCode = exists ? HttpStatusCode.OK : HttpStatusCode.NotFound,
            Message = exists ? "Role exists." : "Role does not exist."
        };
    }
    public async Task<BaseResponse<List<string>>> GetAllRolesAsync()
    {
        var roles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
        return new BaseResponse<List<string>>
        {
            Data = roles,
            StatusCode = HttpStatusCode.OK,
            Message = "Roles retrieved successfully."
        };
    }
    public async Task<BaseResponse<object>> DeleteRoleAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName.ToLower());
        if (role is null)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "Role not found."
            };
        }

        var result = await _roleManager.DeleteAsync(role);
        if (result.Succeeded)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.NoContent,
                Message = "Role deleted successfully."
            };
        }

        return new BaseResponse<object>
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Message = "Failed to delete role."
        };
    }
    public async Task<BaseResponse<object>> AddUserToRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "User not found."
            };
        }

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "Role not found."
            };
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (result.Succeeded)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.OK,
                Message = $"User added to role {roleName} successfully."
            };
        }

        return new BaseResponse<object>
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Message = "Failed to add user to role."
        };
    }
    public async Task<BaseResponse<object>> RemoveUserFromRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "User not found."
            };
        }

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);
        if (result.Succeeded)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.OK,
                Message = $"User removed from role {roleName} successfully."
            };
        }

        return new BaseResponse<object>
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Message = "Failed to remove user from role."
        };
    }
}
