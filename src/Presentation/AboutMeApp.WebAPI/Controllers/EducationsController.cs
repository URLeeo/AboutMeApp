using AboutMeApp.Application.Abstractions.Services;
using AboutMeApp.Application.Dtos.Certificate;
using AboutMeApp.Application.Dtos.Education;
using Microsoft.AspNetCore.Mvc;

namespace AboutMeApp.WebAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class EducationsController : ControllerBase
{
    private IEducationService _educationService { get; }
    public EducationsController(IEducationService educationService)
    {
        _educationService = educationService ?? throw new ArgumentNullException(nameof(educationService));
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] bool isPaginated = true)
    {
        var response = await _educationService.GetAllAsync(pageNumber, pageSize, isPaginated);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _educationService.GetByIdAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchByName(
    [FromQuery] string name,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] bool isPaginated = true)
    {
        var response = await _educationService.SearchByNameAsync(name, pageNumber, pageSize, isPaginated);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EducationCreateDto educationCreateDto)
    {
        var response = await _educationService.CreateAsync(educationCreateDto);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] EducationUpdateDto educationUpdateDto)
    {
        var response = await _educationService.UpdateAsync(id, educationUpdateDto);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _educationService.DeleteAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }
}

