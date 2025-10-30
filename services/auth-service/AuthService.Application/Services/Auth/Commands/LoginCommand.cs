using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs;

public sealed class LoginCommand : ICommand<UserDto>
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}