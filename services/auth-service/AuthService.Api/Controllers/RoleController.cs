using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using AuthService.Application.DTOs;
using AuthService.Application.Services.Role.Commands;
using AuthService.Application.Services.Role.Interfaces;
using AuthService.Infrastructure.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        // Command Dispatcher (Commands)
        private readonly ICommandDispatcher _commands;
        // RabbitMQ Publisher
        private readonly IMessageBusPublisher _messageBusPublisher;

        public RoleController(ICommandDispatcher commands, IMessageBusPublisher messageBusPublisher)
        {
            _commands = commands;
            _messageBusPublisher = messageBusPublisher;
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

            return Ok(res);
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
            return Ok(res);
        }
    }
}
