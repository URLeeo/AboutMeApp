using AboutMeApp.Application.Abstractions.Services;
using AboutMeApp.Application.Dtos.Certificate;
using Microsoft.AspNetCore.Mvc;

namespace AboutMeApp.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CertificatesController : ControllerBase
    {
        private ICertificateService _certificateService { get; }
        public CertificatesController(ICertificateService certificateService)
        {
            _certificateService = certificateService ?? throw new ArgumentNullException(nameof(certificateService));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool isPaginated = true)
        {
            var response = await _certificateService.GetAllAsync(pageNumber, pageSize, isPaginated);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _certificateService.GetByIdAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByName(
        [FromQuery] string name,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool isPaginated = true)
        {
            var response = await _certificateService.SearchByNameAsync(name, pageNumber, pageSize, isPaginated);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CertificateCreateDto certificateCreateDto)
        {
            var response = await _certificateService.CreateAsync(certificateCreateDto);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CertificateUpdateDto certificateUpdateDto)
        {
            var response = await _certificateService.UpdateAsync(id, certificateUpdateDto);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _certificateService.DeleteAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
