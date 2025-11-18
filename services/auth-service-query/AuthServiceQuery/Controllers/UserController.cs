using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Users.Queries.GetUserById;
using AuthService.Application.Users.Queries.GetUserByUsername;
using AuthService.Application.Users.Queries.SearchUsers;
using AuthService.Infrastructure.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace AuthService.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // Command Dispatcher (Commands)
        // RabbitMQ Publisher
        private readonly IMessageBusPublisher _messageBusPublisher;
        // Query Dispatcher (Queries)
        private readonly IQueryDispatcher _queries;

        public UserController(IMessageBusPublisher messageBusPublisher, IQueryDispatcher queries)
        {
            _messageBusPublisher = messageBusPublisher;
            _queries = queries;
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile(CancellationToken ct)
        {
            // JWT token uses "sub" (JwtRegisteredClaimNames.Sub) for user ID
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
                           ?? User.FindFirst("sub") 
                           ?? User.Claims.FirstOrDefault(c => c.Type == "user_id");
            
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(ApiResponse<UserReadDto>.FailureResponse("Invalid or missing user token", 401));
            }
            var res = await _queries.Query(new GetUserByIdQuery(userId), ct);
            if (!res.Success) 
                return NotFound(res);
            return Ok(res);
        }

        // GET: /api/v1/user/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var res = await _queries.Query(new GetUserByIdQuery(id), ct);
            if (!res.Success) 
                return NotFound(res);
            return Ok(res);
        }

        // GET: /api/v1/user/by-username/{username}
        [HttpGet("by-username/{username}")]
        public async Task<IActionResult> GetByUsername(string username, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(ApiResponse<UserReadDto>.FailureResponse("Username cannot be empty", 400));
            }
            var res = await _queries.Query(new GetUserByUsernameQuery(username), ct);
            if (!res.Success) 
                return NotFound(res);
            return Ok(res);
        }

        // GET: /api/v1/user?q=abc&page=1&pageSize=20
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var res = await _queries.Query(new SearchUsersQuery(q, page, pageSize), ct);
            return Ok(res);
        }
    }
}