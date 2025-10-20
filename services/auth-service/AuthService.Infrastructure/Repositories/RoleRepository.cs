using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.DAO;
using AuthService.Infrastructure.DAO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IRoleDAO _dao;
        public RoleRepository(IRoleDAO dao) => _dao = dao;

        public Task<UserRole?> GetByIdAsync(Guid id) => _dao.GetByIdAsync(id);
        public Task<List<UserRole>> GetAllAsync() => _dao.GetAllAsync();
        public Task<UserRole?> GetByNameAsync(string name) => _dao.GetByNameAsync(name);
    }
}
