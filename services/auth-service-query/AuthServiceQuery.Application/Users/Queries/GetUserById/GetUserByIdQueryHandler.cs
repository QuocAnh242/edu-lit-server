using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Users.Queries.GetUserById
{
    public sealed class GetUserByIdQueryHandler
        : IQueryHandler<GetUserByIdQuery, UserReadDto>
    {
        private readonly IUserReadRepository _repository;

        public GetUserByIdQueryHandler(IUserReadRepository repository) => _repository = repository;

        public async Task<ApiResponse<UserReadDto>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            var u = await _repository.GetByIdAsync(query.Id);
            if (u is null) return ApiResponse<UserReadDto>.FailureResponse("User not found", 404);

            var dto = new UserReadDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                RoleId = u.RoleId,
                RoleName = u.RoleName,
                CreatedAt = u.CreatedAt
            };
            return ApiResponse<UserReadDto>.SuccessResponse(dto);
        }
    }
}