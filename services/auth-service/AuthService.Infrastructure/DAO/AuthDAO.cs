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
            // Check for existing username
            var existingByUsername = await _db.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
            if (existingByUsername != null)
            {
                throw new AuthService.Application.Exceptions.AuthException(
                    AuthService.Application.Enums.AuthErrorCode.UserAlreadyExists,
                    $"Username '{username}' is already taken");
            }

            // Check for existing email
            var existingByEmail = await _db.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (existingByEmail != null)
            {
                throw new AuthService.Application.Exceptions.AuthException(
                    AuthService.Application.Enums.AuthErrorCode.UserAlreadyExists,
                    $"Email '{email}' is already registered");
            }

            var defaultRoleName = RoleType.STUDENT.ToString();
            // Get the first STUDENT role (in case there are duplicates, we'll use the first one)
            // TODO: Fix duplicate roles issue - should only have one STUDENT role
            var studentRole = await _db.UserRoles
                .Where(r => r.Name == defaultRoleName)
                .OrderBy(r => r.Id) // Use the first created role
                .FirstOrDefaultAsync();

            if (studentRole == null)
                throw new Exception($"Default role '{defaultRoleName}' not found. Please ensure roles are seeded.");

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

        public async Task<Oauthaccount?> GetOAuthAccountAsync(string provider, string providerAccountId)
        {
            return await _db.Oauthaccounts
                .Include(o => o.User)
                .ThenInclude(u => u!.Role)
                .FirstOrDefaultAsync(o => o.Provider == provider && o.ProviderAccountId == providerAccountId);
        }

        public async Task AddOAuthAccountAsync(Oauthaccount oauthAccount)
        {
            _db.Oauthaccounts.Add(oauthAccount);
            await _uow.SaveChangesAsync();
        }
    }
}
