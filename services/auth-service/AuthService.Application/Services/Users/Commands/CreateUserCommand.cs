using AuthService.Application.Abstractions.Messaging;

namespace AuthService.Application.Services.Users.Commands;

public sealed class CreateUserCommand : ICommand<Guid>
{
    public string Username { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? FullName { get; init; }

    public string Password { get; init; } = null!;
    public Guid? RoleId { get; init; }
}