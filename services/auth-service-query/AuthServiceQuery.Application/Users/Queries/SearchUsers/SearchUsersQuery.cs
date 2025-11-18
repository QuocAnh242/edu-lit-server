using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;

namespace AuthService.Application.Users.Queries.SearchUsers
{
    public sealed record SearchUsersQuery(
        string? Q,
        int Page = 1,
        int PageSize = 20) : IQuery<PagedResult<UserReadDto>>;
}