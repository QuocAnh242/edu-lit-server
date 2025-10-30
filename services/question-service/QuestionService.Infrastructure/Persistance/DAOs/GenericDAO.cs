using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.IDAOs;
using QuestionService.Infrastructure.Data;
using System.Linq.Expressions;

namespace QuestionService.Infrastructure.Persistance.DAOs
{
    public class GenericDAO<T> : IGenericDAO<T> where T : class
    {
        private readonly QuestionDbContext _dbcontext;
        private readonly DbSet<T> _dbset;

        public GenericDAO(QuestionDbContext context)
        {
            _dbcontext = context;
            _dbset = _dbcontext.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbset.AddAsync(entity);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbset.AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbset.FindAsync(id);
        }

        public async Task<PagedResult<T>> GetPagedAsync(int pageIndex, int pageSize, Expression<Func<T, bool>>? filter)
        {
            pageIndex = Math.Max(1, pageIndex);
            pageSize = Math.Max(1, pageSize);

            IQueryable<T> query = _dbset.AsNoTracking();
            if (filter is not null)
            {
                query = query.Where(filter);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageIndex,
                PageSize = pageSize
            };
        }

        public void Remove(T entity)
        {
            _dbset.Remove(entity);
        }

        public void Update(T entity)
        {
            _dbset.Update(entity);
        }
    }
}






