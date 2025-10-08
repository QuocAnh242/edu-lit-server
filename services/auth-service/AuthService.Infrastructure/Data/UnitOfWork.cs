using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Repositories;
using AuthService.Infrastructure.DAO;

namespace AuthService.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AuthDbContext _context;

        public IAuthRepository AuthRepository { get; }

        public UnitOfWork(AuthDbContext context)
        {
            _context = context;
            var authDao = new AuthDAO(_context);
            AuthRepository = new AuthRepository(authDao); 
            // Ensure AuthRepository implements IAuthRepository
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
