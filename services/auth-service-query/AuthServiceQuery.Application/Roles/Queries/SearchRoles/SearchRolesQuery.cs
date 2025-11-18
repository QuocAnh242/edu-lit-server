using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;

namespace AuthService.Application.Roles.Queries.SearchRoles
{
    public sealed record SearchRolesQuery(string? Q, int Page = 1, int PageSize = 20) : IQuery<PagedResult<RoleDto>>;
}

