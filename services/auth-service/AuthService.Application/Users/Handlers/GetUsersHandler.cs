using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Users.Queries;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Users.Handlers;

public sealed class GetUsersHandler(IUserRepository repo) : IQueryHandler<GetUsersQuery, List<UserDto>>
{
    public async Task<ApiResponse<List<UserDto>>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        var users = await repo.GetAllAsync();
        var dtos = users.Select(u => new UserDto(u)).ToList();
        return ApiResponse<List<UserDto>>.SuccessResponse(dtos, "Get All Users Successfully!");
    }
}