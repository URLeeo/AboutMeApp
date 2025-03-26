using AboutMeApp.Application.Abstractions.Services;
using AboutMeApp.Application.Dtos.Template;
using Microsoft.AspNetCore.Mvc;

namespace AboutMeApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplatesController : ControllerBase
    {
        private ITemplateService _templateService { get; }

        public TemplatesController(ITemplateService templateService)
        {
            _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool isPaginated = true)
        {
            var response = await _templateService.GetAllAsync(pageNumber, pageSize, isPaginated);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _templateService.GetByIdAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByName(
            [FromQuery] string name,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool isPaginated = true)
        {
            var response = await _templateService.SearchByNameAsync(name, pageNumber, pageSize, isPaginated);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TemplateCreateDto templateCreateDto)
        {
            var response = await _templateService.CreateAsync(templateCreateDto);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TemplateUpdateDto templateUpdateDto)
        {
            var response = await _templateService.UpdateAsync(id, templateUpdateDto);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _templateService.DeleteAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
