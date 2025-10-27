using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs;
using AuthService.Application.Users.Commands;
using AuthService.Application.Users.Queries;
using AuthService.Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AuthService.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ICommandDispatcher _commands;
        private readonly IQueryDispatcher _queries;
        private readonly IMessageBusPublisher _messageBus;

        public UserController(ICommandDispatcher commands, IQueryDispatcher queries, IMessageBusPublisher messageBus)
        {
            _commands = commands;
            _queries = queries;
            _messageBus = messageBus;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var res = await _queries.Query<GetUsersQuery, List<UserDto>>(new GetUsersQuery(), ct);
            return Ok(res);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken ct = default)
        {
            page = page < 1 ? 1 : page;
            size = size < 1 ? 20 : size > 100 ? 100 : size;

            var res = await _queries.Query<GetUsersPagedQuery, AuthService.Application.DTOs.Response.PageResponse<UserDto>>(
                new GetUsersPagedQuery(page, size), ct);
            return Ok(res);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var res = await _queries.Query<GetUserByIdQuery, UserDto>(new GetUserByIdQuery(id), ct);
            if (!res.Success) return NotFound(res);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDto dto, CancellationToken ct)
        {
            var cmd = new CreateUserCommand
            {
                Username = dto.Username,
                Email = dto.Email,
                FullName = dto.FullName,
                RoleId = dto.RoleId
            };

            var res = await _commands.Send<CreateUserCommand, Guid>(cmd, ct);
            if (!res.Success) return BadRequest(res);

            var evt = new
            {
                id = res.Data,
                username = dto.Username,
                email = dto.Email,
                fullName = dto.FullName,
                roleId = dto.RoleId,
                occurredAt = DateTimeOffset.UtcNow
            };
            await _messageBus.PublishAsync("user.created", JsonSerializer.Serialize(evt), ct);

            return CreatedAtAction(nameof(GetById), new { id = res.Data }, res);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserDto dto, CancellationToken ct)
        {
            var cmd = new UpdateUserCommand(id)
            {
                Username = dto.Username,
                Email = dto.Email,
                FullName = dto.FullName,
                RoleId = dto.RoleId
            };

            var res = await _commands.Send<UpdateUserCommand, bool>(cmd, ct);
            if (!res.Success) return NotFound(res);

            var evt = new
            {
                id,
                username = dto.Username,
                email = dto.Email,
                fullName = dto.FullName,
                roleId = dto.RoleId,
                occurredAt = DateTimeOffset.UtcNow
            };
            await _messageBus.PublishAsync("user.updated", JsonSerializer.Serialize(evt), ct);

            return Ok(res);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var cmd = new DeleteUserCommand(id);
            var res = await _commands.Send<DeleteUserCommand, bool>(cmd, ct);

            if (res.Success)
            {
                var evt = new
                {
                    id,
                    occurredAt = DateTimeOffset.UtcNow
                };
                await _messageBus.PublishAsync("user.deleted", JsonSerializer.Serialize(evt), ct);
            }

            return Ok(res);
        }
    }
}