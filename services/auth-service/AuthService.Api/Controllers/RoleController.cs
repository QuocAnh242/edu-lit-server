using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using AuthService.Application.DTOs;
using AuthService.Application.Services.Role.Commands;
using AuthService.Application.Services.Role.Interfaces;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "TEACHER,ADMIN")]
    public class RoleController : ControllerBase
    {
        // Command Dispatcher (Commands)
        private readonly ICommandDispatcher _commands;
        // RabbitMQ Publisher
        private readonly IMessageBusPublisher _messageBusPublisher;
        private readonly IOutbox _outbox;

        public RoleController(ICommandDispatcher commands, IMessageBusPublisher messageBusPublisher, IOutbox outbox)
        {
            _commands = commands;
            _messageBusPublisher = messageBusPublisher;
            _outbox = outbox;
        }

        // Create Role
        // api/v1/role
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleDto dto, CancellationToken ct)
        {
            var cmd = new CreateRoleCommand
            {
                Name = dto.Name
            };

            var res = await _commands.Send<CreateRoleCommand, Guid>(cmd, ct);
            if (!res.Success) return BadRequest(res);

            return StatusCode(StatusCodes.Status201Created, res);
        }

        // Update Role
        // api/v1/role/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] RoleDto dto, CancellationToken ct)
        {
            var cmd = new UpdateRoleCommand(id)
            {
                Name = dto.Name,
            };

            var res = await _commands.Send<UpdateRoleCommand, bool>(cmd, ct);
            if (!res.Success) return NotFound(res);

            return Ok(res);
        }

        // Delete Role
        // api/v1/role/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var cmd = new DeleteRoleCommand(id);
            var res = await _commands.Send<DeleteRoleCommand, bool>(cmd, ct);
            if (!res.Success) return NotFound(res);

            return NoContent();
        }

        // Sync Roles to Query Service
        // api/v1/role/sync
        [HttpPost("sync")]
        public async Task<IActionResult> SyncRoles([FromServices] IRoleService roleService, [FromServices] IUnitOfWork unitOfWork, CancellationToken ct)
        {
            var rolesResult = await roleService.GetAllAsync();
            if (!rolesResult.Success)
            {
                return BadRequest(rolesResult);
            }

            var synced = 0;
            foreach (var role in rolesResult.Data!)
            {
                await _outbox.EnqueueAsync("auth.role.created", new
                {
                    id = role.Id,
                    name = role.Name,
                }, ct);
                synced++;
            }
            
            // Save outbox messages
            await unitOfWork.SaveChangesAsync(ct);

            return Ok(new { message = $"Synced {synced} roles to query service", count = synced });
        }
    }
}
