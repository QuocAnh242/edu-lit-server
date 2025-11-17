using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs;

namespace AuthService.Application.Roles.Queries.GetRoleById
{
    public sealed record GetRoleByIdQuery(Guid Id) : IQuery<RoleDto>;
}

