using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs;

namespace AuthService.Application.Users.Queries.GetUserRole
{
    public sealed record GetUserRoleQuery(Guid UserId) : IQuery<RoleDto>;
}

