//using AuthService.Application.Services.Role.Interfaces;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using AuthService.Application.Abstractions.Messaging;
//using AuthService.Application.Abstractions.Messaging.Dispatcher.Interfaces;
//using AuthService.Application.DTOs;
//using AuthService.Application.Services.Role.Commands;

//namespace AuthService.Api.Controllers
//{
//    [Route("api/v1/[controller]")]
//    [ApiController]
//    public class RoleController : ControllerBase
//    {
//        private readonly ICommandDispatcher _commands;

//        public RoleController(ICommandDispatcher commands)
//        {
//            _commands = commands;
//        }

//        // Create Role
//        [HttpPost]
//        public async Task<IActionResult> Create([FromBody] RoleDto dto, CancellationToken ct)
//        {
//            var cmd = new CreateRoleCommand
//            {
//                Name = dto.Name,
//            };

//            var res = await _commands.Send<CreateRoleCommand, int>(cmd, ct);
//            if (!res.Success) return BadRequest(res);

//            return Ok(res);
//        }

//        // Update Role
//        [HttpPut("{id:guid}")]
//        public async Task<IActionResult> Update(Guid id, [FromBody] RoleDto dto, CancellationToken ct)
//        {
//            var cmd = new UpdateRoleCommand(id)
//            {
//                Name = dto.Name,
//                Description = dto.Description
//            };

//            var res = await _commands.Send<UpdateRoleCommand, bool>(cmd, ct);
//            if (!res.Success) return NotFound(res);

//            return Ok(res);
//        }

//        // Delete Role
//        [HttpDelete("{id:guid}")]
//        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
//        {
//            var cmd = new DeleteRoleCommand(id);
//            var res = await _commands.Send<DeleteRoleCommand, bool>(cmd, ct);
//            return Ok(res);
//        }
//    }
//}
