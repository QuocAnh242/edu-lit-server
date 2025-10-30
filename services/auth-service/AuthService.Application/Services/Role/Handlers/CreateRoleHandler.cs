using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Services.Role.Commands;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Services.Role.Handlers
{
    public class CreateRoleHandler(IRoleRepository roleRepository, IOutbox outbox) : ICommandHandler<CreateRoleCommand, Guid>
    {
        public async Task<ApiResponse<Guid>> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
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
            }, cancellationToken);

            return ApiResponse<Guid>.SuccessResponse(role.Id, $"Create Role {role.Name} Successfully!");
        }
    }
}
