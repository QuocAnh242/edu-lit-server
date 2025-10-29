using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Services.Role.Commands;
using AuthService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services.Role.Handlers
{
    public class CreateRoleHandler(IRoleRepository roleRepository, IOutbox outbox) : ICommandHandler<CreateRoleCommand, int>
    {
        public async Task<ApiResponse<int>> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
        {
            var role = new Domain.Entities.UserRole
            {
                Name = command.Name,
            };
            await roleRepository.AddAsync(role);
            await outbox.EnqueueAsync("auth.role.created", new
            {
                role.Id,
                role.Name,
                role.CreatedAt
            }, cancellationToken);
            return ApiResponse<int>.SuccessResponse(role.Id, $"Create Role {role.Name} Successfully!");
        }
    }
}
