using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs.Response;
using AuthService.Application.DTOs;

namespace AuthService.Application.Users.Queries;

public sealed class GetUsersPagedQuery(int page, int size) : IQuery<PageResponse<UserDto>>
{
    public int Page { get; } = page;
    public int Size { get; } = size;
}