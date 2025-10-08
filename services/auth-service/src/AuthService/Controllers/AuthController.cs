using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Validate credentials, generate JWT token, etc.
            // For demonstration, return a dummy token.
            return Ok(new { token = "dummy-jwt-token" });
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            // Return user profile info
            return Ok(new { user = User.Identity?.Name });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}