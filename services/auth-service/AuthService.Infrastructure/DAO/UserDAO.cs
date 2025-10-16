using AuthService.Domain.Entities;
using AuthService.Infrastructure.DAO.Interfaces;
using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.DAO
{
    public class UserDAO : IUserDAO
    {
        private readonly AuthDbContext _context;

        public UserDAO(AuthDbContext authDbContext)
        {
            this._context = authDbContext;
        }
        public async Task<User> GetByIdAsync(Guid id) => await _context.Set<User>().FindAsync(id);

        public async Task<List<User>> GetAllAsync() => await _context.Set<User>().ToListAsync();

        public async Task AddAsync(User user)
        {
            _context.Set<User>().Add(user); 
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Set<User>().Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                _context.Set<User>().Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
