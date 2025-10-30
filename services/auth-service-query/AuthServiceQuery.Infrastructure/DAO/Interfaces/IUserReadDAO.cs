using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthService.Domain.Entities.ReadModels;

namespace AuthService.Infrastructure.DAO.Interfaces
{
    public interface IUserReadDAO
    {
        Task<UserReadModel?> GetByIdAsync(Guid id);
        Task<List<UserReadModel>> GetAllAsync();
        Task<(List<UserReadModel> Items, int TotalCount)> GetPagedAsync(int page, int size);
        Task UpsertAsync(UserReadModel model);
        Task DeleteAsync(Guid id);
    }
}