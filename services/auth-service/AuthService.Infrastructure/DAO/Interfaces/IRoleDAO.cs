using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.DAO.Interfaces
{
    public interface IRoleDAO
    {
        Task<UserRole?> GetByIdAsync(Guid id);
        Task<UserRole?> GetByNameAsync(string name);
        Task<List<UserRole>> GetAllAsync();
    }
}
