using AuthService.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Roles.Queries.GetAllRoles;
using AuthService.Application.Roles.Queries.GetRoleById;
using AuthService.Application.Roles.Queries.GetRoleByName;
using AuthService.Application.Roles.Queries.SearchRoles;
using AuthService.Application.Users.Queries.GetUserRole;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IQueryDispatcher _queries;

        public RoleController(IQueryDispatcher queries)
        {
            _queries = queries;
        }

        // GET: /api/v1/role
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var res = await _queries.Query(new GetAllRolesQuery(), ct);
            return Ok(res);
        }

        // GET: /api/v1/role/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var res = await _queries.Query(new GetRoleByIdQuery(id), ct);
            if (!res.Success)
                return NotFound(res);
            return Ok(res);
        }

        // GET: /api/v1/role/by-name/{name}
        [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetByName(string name, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(ApiResponse<RoleDto>.FailureResponse("Role name cannot be empty", 400));
            }
            var res = await _queries.Query(new GetRoleByNameQuery(name), ct);
            if (!res.Success)
                return NotFound(res);
            return Ok(res);
        }

        // GET: /api/v1/role/search?q=abc&page=1&pageSize=20
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var res = await _queries.Query(new SearchRolesQuery(q, page, pageSize), ct);
            return Ok(res);
        }

        // GET: /api/v1/role/user/{userId}
        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetUserRole(Guid userId, CancellationToken ct)
        {
            var res = await _queries.Query(new GetUserRoleQuery(userId), ct);
            if (!res.Success)
                return NotFound(res);
            return Ok(res);
        }
    }
}
