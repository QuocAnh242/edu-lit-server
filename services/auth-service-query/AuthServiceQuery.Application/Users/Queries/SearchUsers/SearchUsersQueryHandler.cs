using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Domain.Entities.ReadModels;
using AuthService.Infrastructure.DAO.Interfaces;
using MongoDB.Driver;

namespace AuthService.Application.Users.Queries.SearchUsers
{
    public sealed class SearchUsersQueryHandler
        : IQueryHandler<SearchUsersQuery, PagedResult<UserReadDto>>
    {
        private readonly IUserReadDAO _dao;

        public SearchUsersQueryHandler(IUserReadDAO dao) => _dao = dao;

        public async Task<ApiResponse<PagedResult<UserReadDto>>> Handle(SearchUsersQuery query, CancellationToken cancellationToken)
        {
            var page = query.Page <= 0 ? 1 : query.Page;
            var size = query.PageSize <= 0 ? 20 : query.PageSize;

            if (_dao is MongoUserReadDAO mongoDao)
            {
                var col = typeof(MongoUserReadDAO)
                    .GetField("_col", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                    .GetValue(mongoDao) as IMongoCollection<UserReadModel>;

                var filter = string.IsNullOrWhiteSpace(query.Q)
                    ? Builders<UserReadModel>.Filter.Empty
                    : Builders<UserReadModel>.Filter.Or(
                        Builders<UserReadModel>.Filter.Regex(x => x.Username, new BsonRegularExpression(query.Q, "i")),
                        Builders<UserReadModel>.Filter.Regex(x => x.Email, new BsonRegularExpression(query.Q, "i")),
                        Builders<UserReadModel>.Filter.Regex(x => x.FullName, new BsonRegularExpression(query.Q, "i"))
                    );

                var total = await col!.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
                var items = await col.Find(filter)
                    .SortBy(x => x.Username)
                    .Skip((page - 1) * size)
                    .Limit(size)
                    .ToListAsync(cancellationToken);

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

            // Fallback (not ideal)
            var (itemsFallback, totalFallback) = await _dao.GetPagedAsync(page, size);
            var dtoItemsFallback = itemsFallback.Select(x => new UserReadDto
            {
                Id = x.Id, Username = x.Username, Email = x.Email, FullName = x.FullName,
                RoleId = x.RoleId, RoleName = x.RoleName, CreatedAt = x.CreatedAt
            }).ToList();

            var resultFallback = new PagedResult<UserReadDto>
            {
                Items = dtoItemsFallback,
                Page = page,
                PageSize = size,
                TotalItems = totalFallback
            };

            return ApiResponse<PagedResult<UserReadDto>>.SuccessResponse(resultFallback);
        }
    }
}