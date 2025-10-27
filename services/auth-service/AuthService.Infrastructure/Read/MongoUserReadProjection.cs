using System;
using System.Threading.Tasks;
using AuthService.Application.Services.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Entities.ReadModels;
using AuthService.Infrastructure.DAO.Interfaces;

namespace AuthService.Infrastructure.Read
{
    public sealed class MongoUserReadProjection : IUserReadProjection
    {
        private readonly IUserReadDAO _readDao;

        public MongoUserReadProjection(IUserReadDAO readDao)
        {
            _readDao = readDao;
        }

        public async Task UpsertAsync(User user)
        {
            var model = new UserReadModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name,
                CreatedAt = user.CreatedAt
            };
            await _readDao.UpsertAsync(model);
        }

        public Task DeleteAsync(Guid id) => _readDao.DeleteAsync(id);
    }
}