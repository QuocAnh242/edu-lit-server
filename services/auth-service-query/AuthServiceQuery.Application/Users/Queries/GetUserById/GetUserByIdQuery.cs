using AuthService.Application.Abstractions.Messaging;

namespace AuthService.Application.Users.Queries.GetUserById
{
    public sealed record GetUserByIdQuery(Guid Id) : IQuery<AuthService.Application.DTOs.UserReadDto>;
}