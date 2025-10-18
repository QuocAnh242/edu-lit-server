using AuthService.Application.Enums;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.DAO.Interfaces;
using AuthService.Infrastructure.Data;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.DAO
{
    public class AuthDAO : IAuthDAO
    {
        private readonly AuthDbContext _dbcontext;
        public AuthDAO(AuthDbContext context)
        {
            _dbcontext = context;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            return await _dbcontext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }

        public async Task<User> RegisterAsync(string username, string email, string password, string fullName)
        {
            var defaultRoleName = RoleType.STUDENT.ToString();
            var studentRole = await _dbcontext.UserRoles.FirstOrDefaultAsync(r => r.Name == defaultRoleName);

            if (studentRole == null)
                throw new Exception($"Default role '{defaultRoleName}' not found.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                Password = password,
                FullName = fullName,
                CreatedAt = DateTime.UtcNow,
                RoleId = studentRole.Id,
                Role = studentRole // Ensure RoleName is available without requery
            };

            _dbcontext.Users.Add(user);
            await _dbcontext.SaveChangesAsync();

            // Alternatively, re-load with Include if you prefer:
            // user = await _dbcontext.Users.Include(u => u.Role).FirstAsync(u => u.Id == user.Id);

            return user;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbcontext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
