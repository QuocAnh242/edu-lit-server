using AuthService.Application.DTOs.Request;
using AuthService.Application.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AuthService.Application.DTOs.Response;
using System.Threading;
using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.Abstractions.Messaging.Dispatcher.Interfaces;

namespace AuthService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ICommandDispatcher _commands;

        public AuthController(ICommandDispatcher commands)
        {
            _commands = commands;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<UserDto>.FailureResponse("Invalid input", 400));

            var cmd = new LoginCommand
            {
                Username = request.Username,
                Password = request.Password
            };

            var response = await _commands.Send<LoginCommand, UserDto>(cmd, ct);
            if (!response.Success)
                return Unauthorized(response);

            return Ok(response);
        }
            
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<UserDto>.FailureResponse("Invalid input", 400));

            var cmd = new RegisterCommand
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                FullName = request.FullName
            };

            var response = await _commands.Send<RegisterCommand, UserDto>(cmd, ct);
            if (!response.Success || response.Data == null)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
