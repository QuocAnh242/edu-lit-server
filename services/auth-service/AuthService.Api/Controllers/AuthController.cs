using AuthService.Application.DTOs.Request;
using AuthService.Application.DTOs;
using AuthService.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AuthService.Application.DTOs.Response;

namespace AuthService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<UserDto>.FailureResponse("Invalid input", 400));

            var response = await _authService.LoginAsync(
                new LoginCommand
                {
                    Username = request.Username,
                    Password = request.Password
                });

            if (!response.Success)
                return Unauthorized(response);

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<UserDto>.FailureResponse("Invalid input", 400));

            var command = new RegisterCommand
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                FullName = request.FullName
            };

            var response = await _authService.RegisterAsync(command);
            if (!response.Success || response.Data == null)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
