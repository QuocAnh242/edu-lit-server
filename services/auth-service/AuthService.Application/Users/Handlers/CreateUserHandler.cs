using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Users.Commands;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Users.Handlers;

public sealed class CreateUserHandler(IUserRepository repo, IOutbox outbox) : ICommandHandler<CreateUserCommand, Guid>
{
    public async Task<ApiResponse<Guid>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = command.Username,
            Email = command.Email,
            FullName = command.FullName,
            RoleId = command.RoleId,
            Password = string.Empty, // No password on admin-created user unless you add flow
            CreatedAt = DateTime.UtcNow
        };

        await repo.AddAsync(user);

        await outbox.EnqueueAsync("auth.user.created", new
        {
            user.Id,
            user.Username,
            user.Email,
            user.FullName,
            user.RoleId,
            user.CreatedAt
        }, cancellationToken);

        return ApiResponse<Guid>.SuccessResponse(user.Id, "Create User Successfully!");
    }
}