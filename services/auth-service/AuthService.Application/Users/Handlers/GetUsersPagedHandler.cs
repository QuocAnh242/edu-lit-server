using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Users.Queries;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Users.Handlers;

public sealed class GetUsersPagedHandler(IUserRepository repo) : IQueryHandler<GetUsersPagedQuery, PageResponse<UserDto>>
{
    public async Task<ApiResponse<PageResponse<UserDto>>> Handle(GetUsersPagedQuery query, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repo.GetPagedAsync(query.Page, query.Size);
        var dtoItems = items.Select(u => new UserDto(u)).ToList();
        var totalPages = (int)Math.Ceiling(totalCount / (double)query.Size);

        var pageResponse = new PageResponse<UserDto>
        {
            Content = dtoItems,
            TotalElements = totalCount,
            TotalPages = totalPages,
            Page = query.Page,
            Size = query.Size
        };

        return ApiResponse<PageResponse<UserDto>>.SuccessResponse(pageResponse, "Get Users (Paged) Successfully!");
    }
}