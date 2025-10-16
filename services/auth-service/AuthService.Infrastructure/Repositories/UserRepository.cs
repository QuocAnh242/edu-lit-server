using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.DAO.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IUserDAO _dao;

        public UserRepository(IUserDAO dao) =>_dao = dao;

        public Task<User> GetByIdAsync(Guid id) => _dao.GetByIdAsync(id);
        public Task<List<User>> GetAllAsync() => _dao.GetAllAsync();
        public Task AddAsync(User user) => _dao.AddAsync(user);
        public Task UpdateAsync(User user) => _dao.UpdateAsync(user);
        public Task DeleteAsync(Guid id) => _dao.DeleteAsync(id);
    }
}
