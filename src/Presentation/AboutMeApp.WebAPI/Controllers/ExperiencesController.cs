using AboutMeApp.Application.Abstractions.Services;
using AboutMeApp.Application.Dtos.Education;
using AboutMeApp.Application.Dtos.Experience;
using Microsoft.AspNetCore.Mvc;

namespace AboutMeApp.WebAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ExperiencesController : ControllerBase
{
    private IExperienceService _experienceService { get; }
    public ExperiencesController(IExperienceService experienceService)
    {
        _experienceService = experienceService ?? throw new ArgumentNullException(nameof(experienceService));
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] bool isPaginated = true)
    {
        var response = await _experienceService.GetAllAsync(pageNumber, pageSize, isPaginated);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _experienceService.GetByIdAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchByName(
    [FromQuery] string name,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] bool isPaginated = true)
    {
        var response = await _experienceService.SearchByNameAsync(name, pageNumber, pageSize, isPaginated);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ExperienceCreateDto experienceCreateDto)
    {
        var response = await _experienceService.CreateAsync(experienceCreateDto);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ExperienceUpdateDto experienceUpdateDto)
    {
        var response = await _experienceService.UpdateAsync(id, experienceUpdateDto);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _experienceService.DeleteAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }
}
