using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Users.Commands;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Users.Handlers;

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

        await outbox.EnqueueAsync("auth.user.updated", new
        {
            user.Id,
            user.Username,
            user.Email,
            user.FullName,
            user.RoleId
        }, cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true, "Update User Successfully!");
    }
}