using System.Linq.Expressions;

namespace QuestionService.Domain.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<List<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task SaveChangesAsync();
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
    }
}



