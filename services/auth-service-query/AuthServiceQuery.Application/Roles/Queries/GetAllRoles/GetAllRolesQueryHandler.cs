using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Roles.Queries.GetAllRoles
{
    public sealed class GetAllRolesQueryHandler : IQueryHandler<GetAllRolesQuery, List<RoleDto>>
    {
        private readonly IRoleRepository _repository;

        public GetAllRolesQueryHandler(IRoleRepository repository) => _repository = repository;

        public async Task<ApiResponse<List<RoleDto>>> Handle(GetAllRolesQuery query, CancellationToken cancellationToken)
        {
            var roles = await _repository.GetAllAsync();
            var dtos = roles.Select(r => new RoleDto(r)).ToList();
            return ApiResponse<List<RoleDto>>.SuccessResponse(dtos);
        }
    }
}

