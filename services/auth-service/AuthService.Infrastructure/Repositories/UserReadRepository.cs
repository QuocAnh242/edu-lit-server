using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.DAO.Interfaces;
using AuthService.Domain.Entities.ReadModels;

namespace AuthService.Infrastructure.Repositories
{
    public sealed class UserReadRepository : IUserReadRepository
    {
        private readonly IUserReadDAO _dao;
        public UserReadRepository(IUserReadDAO dao) => _dao = dao;

        public Task<UserReadModel?> GetByIdAsync(Guid id) => _dao.GetByIdAsync(id);
        public Task<List<UserReadModel>> GetAllAsync() => _dao.GetAllAsync();
        public Task<(List<UserReadModel> Items, int TotalCount)> GetPagedAsync(int page, int size) => _dao.GetPagedAsync(page, size);
    }
}