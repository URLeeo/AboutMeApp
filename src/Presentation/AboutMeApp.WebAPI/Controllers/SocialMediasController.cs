using AboutMeApp.Application.Abstractions.Services;
using AboutMeApp.Application.Dtos.Experience;
using AboutMeApp.Application.Dtos.SocialMedia;
using Microsoft.AspNetCore.Mvc;

namespace AboutMeApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocialMediasController : ControllerBase
    {
        private ISocialMediaService _socialmediaService { get; }
        public SocialMediasController(ISocialMediaService socialmediaService)
        {
            _socialmediaService = socialmediaService ?? throw new ArgumentNullException(nameof(socialmediaService));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool isPaginated = true)
        {
            var response = await _socialmediaService.GetAllAsync(pageNumber, pageSize, isPaginated);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _socialmediaService.GetByIdAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByName(
        [FromQuery] string name,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool isPaginated = true)
        {
            var response = await _socialmediaService.SearchByNameAsync(name, pageNumber, pageSize, isPaginated);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SocialMediaCreateDto socialmediaCreateDto)
        {
            var response = await _socialmediaService.CreateAsync(socialmediaCreateDto);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SocialMediaUpdateDto socialmediaUpdateDto)
        {
            var response = await _socialmediaService.UpdateAsync(id, socialmediaUpdateDto);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _socialmediaService.DeleteAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
