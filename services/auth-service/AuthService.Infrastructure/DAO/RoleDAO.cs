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
    public class RoleDAO : IRoleDAO
    {
        private readonly AuthDbContext _context;
        public RoleDAO(AuthDbContext context) => _context = context;

        public async Task<UserRole?> GetByIdAsync(Guid id)
            => await _context.Set<UserRole>().FindAsync(id);

        public async Task<UserRole?> GetByNameAsync(string name)
            => await _context.Set<UserRole>().FirstOrDefaultAsync(r => r.Name == name);

        public async Task<List<UserRole>> GetAllAsync()
            => await _context.Set<UserRole>().ToListAsync();

    }
}
