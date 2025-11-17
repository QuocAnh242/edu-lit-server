using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs;

namespace AuthService.Application.Roles.Queries.GetRoleByName
{
    public sealed record GetRoleByNameQuery(string Name) : IQuery<RoleDto>;
}

