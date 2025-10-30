using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Users.Queries.GetUserByUsername
{
    public sealed class GetUserByUsernameQueryHandler
        : IQueryHandler<GetUserByUsernameQuery, UserReadDto>
    {
        private readonly IUserReadRepository _repository;

        public GetUserByUsernameQueryHandler(IUserReadRepository repository) => _repository = repository;

        public async Task<ApiResponse<UserReadDto>> Handle(GetUserByUsernameQuery query, CancellationToken cancellationToken)
        {
            var u = await _repository.GetByUsernameAsync(query.Username);
            if (u is null) 
                return ApiResponse<UserReadDto>.FailureResponse("User not found", 404);

            return ApiResponse<UserReadDto>.SuccessResponse(new UserReadDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                RoleId = u.RoleId,
                RoleName = u.RoleName,
                CreatedAt = u.CreatedAt
            });
        }
    }
}