using AboutMeApp.Application.Abstractions.Services;
using AboutMeApp.Application.Dtos.Template;
using AboutMeApp.Application.Dtos.UserProfile;
using Microsoft.AspNetCore.Mvc;

namespace AboutMeApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController : ControllerBase
    {
        private IUserProfileService _userprofileService { get; }

        public UserProfilesController(IUserProfileService userprofileService)
        {
            _userprofileService = userprofileService ?? throw new ArgumentNullException(nameof(userprofileService));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool isPaginated = true)
        {
            var response = await _userprofileService.GetAllAsync(pageNumber, pageSize, isPaginated);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _userprofileService.GetByIdAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByName(
            [FromQuery] string name,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool isPaginated = true)
        {
            var response = await _userprofileService.SearchByNameAsync(name, pageNumber, pageSize, isPaginated);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserProfileCreateDto userprofileCreateDto)
        {
            var response = await _userprofileService.CreateAsync(userprofileCreateDto);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserProfileUpdateDto userprofileUpdateDto)
        {
            var response = await _userprofileService.UpdateAsync(id, userprofileUpdateDto);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _userprofileService.DeleteAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
