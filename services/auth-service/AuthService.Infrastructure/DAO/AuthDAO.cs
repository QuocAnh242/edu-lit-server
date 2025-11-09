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

        // Login user by username and password
        public async Task<User?> LoginAsync(string username, string password)
        {
            // Fetch user by username only (password is verified with BCrypt)
            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user is null) return null;

            // Verify password with BCrypt (hash comparison)
            var ok = BCrypt.Net.BCrypt.Verify(password, user.Password);
            return ok ? user : null;
        }

        // Register a new user with default role as STUDENT
        public async Task<User> RegisterAsync(string username, string email, string password, string fullName)
        {
            var defaultRoleName = RoleType.STUDENT.ToString();
            var studentRole = await _db.UserRoles.FirstOrDefaultAsync(r => r.Name == defaultRoleName);

            if (studentRole == null)
                throw new Exception($"Default role '{defaultRoleName}' not found.");

            // Hash the password before storing
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                Password = passwordHash,
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
