using AssessmentService.Domain.IDAOs;
using AssessmentService.Domain.Interfaces;

namespace AssessmentService.Infrastructure.Persistance.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly IGenericDAO<T> _genericDAO;

        public GenericRepository(IGenericDAO<T> genericDAO)
        {
            _genericDAO = genericDAO;
        }

        public async Task AddAsync(T entity)
        {
            await _genericDAO.AddAsync(entity);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _genericDAO.GetAllAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _genericDAO.GetByIdAsync(id);
        }

        public async Task<Domain.Commons.PagedResult<T>> GetPagedAsync(int pageIndex, int pageSize, System.Linq.Expressions.Expression<Func<T, bool>>? filter = null)
        {
            return await _genericDAO.GetPagedAsync(pageIndex, pageSize, filter);
        }

        public void Remove(T entity)
        {
            _genericDAO.Remove(entity);
        }

        public void Update(T entity)
        {
            _genericDAO.Update(entity);
        }
    }
}
