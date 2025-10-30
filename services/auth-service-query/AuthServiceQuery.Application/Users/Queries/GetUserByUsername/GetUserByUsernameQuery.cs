using AuthService.Application.Abstractions.Messaging;

namespace AuthService.Application.Users.Queries.GetUserByUsername
{
    public sealed record GetUserByUsernameQuery(string Username) : IQuery<AuthService.Application.DTOs.UserReadDto>;
}