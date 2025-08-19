using AboutMeApp.Application.Abstractions.Services;
using AboutMeApp.Application.Dtos.Identity;
using AboutMeApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AboutMeApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthsController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<User> _userManager;

        public AuthsController(IAuthService authService, UserManager<User> userManager)
        {
            _authService = authService;
            _userManager = userManager;
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

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Content("<h2 style='color:red;'>❌ Invalid user ID.</h2>", "text/html");

            var decodedToken = WebUtility.UrlDecode(token);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
                return Content("<h2 style='color:green;'>✅ Your email has been confirmed successfully!</h2>", "text/html");

            return Content("<h2 style='color:red;'>❌ Email confirmation failed. The link may have expired or is invalid.</h2>", "text/html");
        }
    }
}
