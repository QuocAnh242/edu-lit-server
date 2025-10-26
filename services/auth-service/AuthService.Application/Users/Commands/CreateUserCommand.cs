using AuthService.Application.Abstractions.Messaging;

namespace AuthService.Application.Users.Commands;

public sealed class CreateUserCommand : ICommand<Guid>
{
    public string Username { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? FullName { get; init; }
    public Guid? RoleId { get; init; }
}