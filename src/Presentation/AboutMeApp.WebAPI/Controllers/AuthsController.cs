using AboutMeApp.Application.Abstractions.Services;
using AboutMeApp.Application.Dtos.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AboutMeApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthsController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthsController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(string refreshtoken)
        {
            var response = await _authService.RefreshToken(refreshtoken);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var response = await _authService.RegisterAsync(registerDto);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
