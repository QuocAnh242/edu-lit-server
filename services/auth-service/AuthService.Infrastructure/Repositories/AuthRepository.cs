using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.DAO;
using AuthService.Domain.Entities;
using System.Threading.Tasks;
using AuthService.Infrastructure.DAO.Interfaces;

namespace AuthService.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IAuthDAO _authDAO;

        public AuthRepository(IAuthDAO authDAO)
        {
            _authDAO = authDAO;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            return await _authDAO.LoginAsync(username, password);
        }

        public async Task<User?> RegisterAsync(string username, string email, string password, string fullName)
        {
            return await _authDAO.RegisterAsync(username, email, password, fullName);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _authDAO.GetByUsernameAsync(username);
        }

        public async Task<Oauthaccount?> GetOAuthAccountAsync(string provider, string providerAccountId)
        {
            return await _authDAO.GetOAuthAccountAsync(provider, providerAccountId);
        }

        public async Task AddOAuthAccountAsync(Oauthaccount oauthAccount)
        {
            await _authDAO.AddOAuthAccountAsync(oauthAccount);
        }
    }
}
