using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.IRepositories;
using QuestionService.Infrastructure.Data;
using System.Linq.Expressions;

namespace QuestionService.Infrastructure.Persistance.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly QuestionDbContext _context;
        private readonly DbSet<T> _dbset;

        public GenericRepository(QuestionDbContext context)
        {
            _context = context;
            _dbset = _context.Set<T>();
        }

        public async Task AddAsync(T entity) => await _dbset.AddAsync(entity);

        public async Task<List<T>> GetAllAsync() => await _dbset.AsNoTracking().ToListAsync();

        public async Task<T?> GetByIdAsync(Guid id) => await _dbset.FindAsync(id);

        public void Remove(T entity) => _dbset.Remove(entity);

        public void Update(T entity) => _dbset.Update(entity);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbset.AsNoTracking().Where(predicate).ToListAsync();
        }
    }
}







