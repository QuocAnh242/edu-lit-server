using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Roles.Queries.GetRoleById
{
    public sealed class GetRoleByIdQueryHandler : IQueryHandler<GetRoleByIdQuery, RoleDto>
    {
        private readonly IRoleRepository _repository;

        public GetRoleByIdQueryHandler(IRoleRepository repository) => _repository = repository;

        public async Task<ApiResponse<RoleDto>> Handle(GetRoleByIdQuery query, CancellationToken cancellationToken)
        {
            var role = await _repository.GetByIdAsync(query.Id);
            if (role is null)
                return ApiResponse<RoleDto>.FailureResponse("Role not found", 404);

            return ApiResponse<RoleDto>.SuccessResponse(new RoleDto(role));
        }
    }
}

