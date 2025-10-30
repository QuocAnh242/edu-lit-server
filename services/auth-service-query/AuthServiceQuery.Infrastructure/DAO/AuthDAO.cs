using AuthService.Application.Enums;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.DAO.Interfaces;
using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.DAO
{
    public class AuthDAO : IAuthDAO
    {
        private readonly IUnitOfWork _uow;
        private readonly AuthDbContext _db;

        public AuthDAO(IUnitOfWork uow)
        {
            _uow = uow;
            _db = (AuthDbContext)_uow.Context;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            return await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }

        public async Task<User> RegisterAsync(string username, string email, string password, string fullName)
        {
            var defaultRoleName = RoleType.STUDENT.ToString();
            var studentRole = await _db.UserRoles.FirstOrDefaultAsync(r => r.Name == defaultRoleName);

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
                Role = studentRole
            };

            _db.Users.Add(user);
            await _uow.SaveChangesAsync();

            return user;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
