using AboutMeApp.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AboutMeApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        [HttpPost("Create")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var response = await _roleService.CreateRoleAsync(roleName);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> GetAllRoles()
        {
            var response = await _roleService.GetAllRolesAsync();
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpDelete]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var response = await _roleService.DeleteRoleAsync(roleName);
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpPost("AddUser")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> AddUserToRole(string userId, string roleName)
        {
            var response = await _roleService.AddUserToRoleAsync(userId, roleName);
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpPost("RemoveUser")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> RemoveUserFromRole(string userId, string roleName)
        {
            var response = await _roleService.RemoveUserFromRoleAsync(userId, roleName);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
