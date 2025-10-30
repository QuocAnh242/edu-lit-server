using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Domain.Entities.ReadModels;
using AuthService.Infrastructure.DAO.Interfaces;
using MongoDB.Driver;

namespace AuthService.Application.Users.Queries.GetUserByUsername
{
    public sealed class GetUserByUsernameQueryHandler
        : IQueryHandler<GetUserByUsernameQuery, UserReadDto>
    {
        private readonly IUserReadDAO _dao;

        public GetUserByUsernameQueryHandler(IUserReadDAO dao) => _dao = dao;

        public async Task<ApiResponse<UserReadDto>> Handle(GetUserByUsernameQuery query, CancellationToken cancellationToken)
        {
            // DAO has no direct "by username" in interface; implement via GetAll or extend DAO if needed.
            // Prefer extending DAO: add GetByUsernameAsync. For now, simple scan is acceptable for small data sets.
            if (_dao is MongoUserReadDAO mongoDao)
            {
                // Efficient path using collection
                var col = typeof(MongoUserReadDAO)
                    .GetField("_col", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                    .GetValue(mongoDao) as IMongoCollection<UserReadModel>;

                var u = await col!.Find(x => x.Username == query.Username).FirstOrDefaultAsync(cancellationToken);
                if (u is null) return ApiResponse<UserReadDto>.FailureResponse("User not found", 404);

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

            // Fallback (not ideal)
            var all = await _dao.GetAllAsync();
            var user = all.FirstOrDefault(x => x.Username == query.Username);
            if (user is null) return ApiResponse<UserReadDto>.FailureResponse("User not found", 404);

            return ApiResponse<UserReadDto>.SuccessResponse(new UserReadDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                RoleId = user.RoleId,
                RoleName = user.RoleName,
                CreatedAt = user.CreatedAt
            });
        }
    }
}