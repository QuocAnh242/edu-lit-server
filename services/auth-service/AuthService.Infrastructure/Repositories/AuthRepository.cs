using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.DAO;
using AuthService.Domain.Entities;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AuthDAO _authDAO;

        public AuthRepository(AuthDAO authDAO)
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
            // Implement a method in AuthDAO to get user by username
            return await _authDAO.GetByUsernameAsync(username);
        }
    }
}
