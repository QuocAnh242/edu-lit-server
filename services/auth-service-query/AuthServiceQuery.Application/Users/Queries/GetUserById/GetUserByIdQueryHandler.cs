using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Domain.Entities.ReadModels;
using AuthService.Infrastructure.DAO.Interfaces;

namespace AuthService.Application.Users.Queries.GetUserById
{
    public sealed class GetUserByIdQueryHandler
        : IQueryHandler<GetUserByIdQuery, UserReadDto>
    {
        private readonly IUserReadDAO _dao;

        public GetUserByIdQueryHandler(IUserReadDAO dao) => _dao = dao;

        public async Task<ApiResponse<UserReadDto>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            var u = await _dao.GetByIdAsync(query.Id);
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