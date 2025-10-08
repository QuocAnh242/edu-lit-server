using AuthService.Infrastructure.Data;
using AuthService.Domain.Entities;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.DAO
{
    public class AuthDAO
    {
        private readonly AuthDbContext _dbcontext;
        public AuthDAO(AuthDbContext context)
        {
            _dbcontext = context;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            return await Task.FromResult(
                _dbcontext.Users.FirstOrDefault(u => u.Username == username && u.Password == password)
            );
        }

        public async Task<User> RegisterAsync(string username, string email, string password, string fullName)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                Password = password,
                FullName = fullName,
                CreatedAt = DateTime.UtcNow
            };
            _dbcontext.Users.Add(user);
            await _dbcontext.SaveChangesAsync();
            return user;
        }
    }
}
