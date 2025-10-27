using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Users.Commands;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Users.Handlers;

public sealed class DeleteUserHandler(IUserRepository repo, IOutbox outbox) : ICommandHandler<DeleteUserCommand, bool>
{
    public async Task<ApiResponse<bool>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        await repo.DeleteAsync(command.Id);

        await outbox.EnqueueAsync("auth.user.deleted", new
        {
            Id = command.Id
        }, cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true, "Delete Successfully!");
    }
}