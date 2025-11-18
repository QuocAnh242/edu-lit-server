using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Services.Users.Commands;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Services.Users.Handlers;

public sealed class CreateUserHandler(IUserRepository repo, IOutbox outbox) : ICommandHandler<CreateUserCommand, Guid>
{
    public async Task<ApiResponse<Guid>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        // Hash password if provided
        var passwordHash = !string.IsNullOrWhiteSpace(command.Password)
            ? BCrypt.Net.BCrypt.HashPassword(command.Password)
            : null;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = command.Username,
            Email = command.Email,
            FullName = command.FullName,
            RoleId = command.RoleId,
            Password = passwordHash ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        await repo.AddAsync(user);

        // Reload user with role to get role name
        user = await repo.GetByIdAsync(user.Id);

        await outbox.EnqueueAsync("auth.user.created", new
        {
            id = user.Id,
            username = user.Username,
            email = user.Email,
            fullName = user.FullName,
            roleId = user.RoleId,
            roleName = user.Role?.Name,
            createdAt = user.CreatedAt
        }, cancellationToken);

        return ApiResponse<Guid>.SuccessResponse(user.Id, $"Create User {user.FullName} Successfully!");
    }
}