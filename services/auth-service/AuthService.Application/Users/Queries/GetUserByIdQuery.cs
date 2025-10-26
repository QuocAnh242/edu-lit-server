using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs;

namespace AuthService.Application.Users.Queries;

public sealed class GetUserByIdQuery(Guid id) : IQuery<UserDto>
{
    public Guid Id { get; } = id;
}