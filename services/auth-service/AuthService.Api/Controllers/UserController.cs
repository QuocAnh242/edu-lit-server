using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using AuthService.Application.DTOs;
using AuthService.Application.Services.Users.Commands;
using AuthService.Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AuthService.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // Command Dispatcher (Commands)
        private readonly ICommandDispatcher _commands;
        // RabbitMQ Publisher
        private readonly IMessageBusPublisher _messageBusPublisher;

        public UserController(ICommandDispatcher commands, IMessageBusPublisher messageBusPublisher)
        {
            _commands = commands;
            _messageBusPublisher = messageBusPublisher;
        }

        // Create Account Method
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDto dto, CancellationToken ct)
        {
            // Mapping from DTO into Command
            var cmd = new CreateUserCommand
            {
                Username = dto.Username,
                Email = dto.Email,
                FullName = dto.FullName,
                RoleId = dto.RoleId
            };

            var res = await _commands.Send<CreateUserCommand, Guid>(cmd, ct);

            // Handle Errors
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
            await _messageBusPublisher.PublishAsync("auth.user.created", JsonSerializer.Serialize(evt), ct);

            // Return Response 
            return Ok(res);
        }

        // Update Account Method
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserDto dto, CancellationToken ct)
        {
            // Mapping from DTO into Command
            var cmd = new UpdateUserCommand(id)
            {
                Username = dto.Username,
                Email = dto.Email,
                FullName = dto.FullName,
                RoleId = dto.RoleId
            };

            var res = await _commands.Send<UpdateUserCommand, bool>(cmd, ct);

            // Handle Errors
            if (!res.Success) return NotFound(res);

            // Create event payload, publish to RabbitMQ
            var evt = new
            {
                id = id,
                username = dto.Username,
                email = dto.Email,
                fullName = dto.FullName,
                roleId = dto.RoleId,
                occurredAt = DateTimeOffset.UtcNow
            };
            await _messageBusPublisher.PublishAsync("auth.user.updated", JsonSerializer.Serialize(evt), ct);

            // Return Response
            return Ok(res);
        }

        // Delete Account Method
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            // Command side only: delete in auth-service-write
            var cmd = new DeleteUserCommand(id);
            
            var res = await _commands.Send<DeleteUserCommand, bool>(cmd, ct);
            
            if(res.Success)
            {
                var evt = new
                {
                    id = id,
                    occurredAt = DateTimeOffset.UtcNow
                };
                await _messageBusPublisher.PublishAsync("auth.user.deleted", JsonSerializer.Serialize(evt), ct);
            }

            return Ok(res);
        }
    }
}