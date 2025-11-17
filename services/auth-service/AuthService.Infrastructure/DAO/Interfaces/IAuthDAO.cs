using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.DAO.Interfaces
{
    public interface IAuthDAO
    {
        Task<User?> LoginAsync(string username, string password);
        Task<User?> RegisterAsync(string username, string email, string password, string fullName);
        Task<User?> GetByUsernameAsync(string username);
        Task<Oauthaccount?> GetOAuthAccountAsync(string provider, string providerAccountId);
        Task AddOAuthAccountAsync(Oauthaccount oauthAccount);
    }
}
