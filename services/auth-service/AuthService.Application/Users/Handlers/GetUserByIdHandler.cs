using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Users.Queries;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Users.Handlers;

public sealed class GetUserByIdHandler(IUserRepository repo) : IQueryHandler<GetUserByIdQuery, UserDto>
{
    public async Task<ApiResponse<UserDto>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await repo.GetByIdAsync(query.Id);
        if (user is null) return ApiResponse<UserDto>.FailureResponse("Not found", 404);
        return ApiResponse<UserDto>.SuccessResponse(new UserDto(user), "Get UserById Successfully!");
    }
}