using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs;

namespace AuthService.Application.Roles.Queries.GetAllRoles
{
    public sealed record GetAllRolesQuery : IQuery<List<RoleDto>>;
}

