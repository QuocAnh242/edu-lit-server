using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Services.Users.Commands;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Services.Users.Handlers;

public sealed class UpdateUserHandler(IUserRepository repo, IOutbox outbox) : ICommandHandler<UpdateUserCommand, bool>
{
    public async Task<ApiResponse<bool>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await repo.GetByIdAsync(command.Id);
        if (user is null) return ApiResponse<bool>.FailureResponse("Not found", 404);

        user.Username = command.Username;
        user.Email = command.Email;
        user.FullName = command.FullName;
        user.RoleId = command.RoleId;

        await repo.UpdateAsync(user);

        // Reload user with role to get role name
        user = await repo.GetByIdAsync(user.Id);

        await outbox.EnqueueAsync("auth.user.updated", new
        {
            id = user.Id,
            username = user.Username,
            email = user.Email,
            fullName = user.FullName,
            roleId = user.RoleId,
            roleName = user.Role?.Name
        }, cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true, "Update User Successfully!");
    }
}