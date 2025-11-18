using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Users.Queries.SearchUsers
{
    public sealed class SearchUsersQueryHandler
        : IQueryHandler<SearchUsersQuery, PagedResult<UserReadDto>>
    {
        private readonly IUserReadRepository _repository;

        public SearchUsersQueryHandler(IUserReadRepository repository) => _repository = repository;

        public async Task<ApiResponse<PagedResult<UserReadDto>>> Handle(SearchUsersQuery query, CancellationToken cancellationToken)
        {
            var page = query.Page <= 0 ? 1 : query.Page;
            var size = query.PageSize <= 0 ? 20 : query.PageSize;

            var (items, total) = await _repository.SearchAsync(query.Q, page, size);

            var dtoItems = items.Select(x => new UserReadDto
            {
                Id = x.Id,
                Username = x.Username,
                Email = x.Email,
                FullName = x.FullName,
                RoleId = x.RoleId,
                RoleName = x.RoleName,
                CreatedAt = x.CreatedAt
            }).ToList();

            var result = new PagedResult<UserReadDto>
            {
                Items = dtoItems,
                Page = page,
                PageSize = size,
                TotalItems = total
            };

            return ApiResponse<PagedResult<UserReadDto>>.SuccessResponse(result);
        }
    }
}