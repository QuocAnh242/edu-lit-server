using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs;

namespace AuthService.Application.Services.Auth.Commands
{
    public sealed record UpdateProfileCommand : ICommand<UserDto>
    {
        public Guid UserId { get; init; }
        public string FullName { get; init; } = null!;
    }
}

