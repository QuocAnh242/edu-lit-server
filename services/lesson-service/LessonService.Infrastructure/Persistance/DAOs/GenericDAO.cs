using LessonService.Domain.Commons;
using LessonService.Domain.IDAOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LessonService.Infrastructure.Persistance.DAOs
{
    // Add 'where T : class' constraint to ensure T is a reference type
    public class GenericDAO<T> : IGenericDAO<T> where T : class 
    {
        private readonly LessonDbContext _dbcontext;
        private readonly DbSet<T> _dbset;

        public GenericDAO(LessonDbContext context)
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
            // Validate and normalize input parameters
            pageIndex = Math.Max(1, pageIndex);
            pageSize = Math.Max(1, pageSize);

            // Create base query
            IQueryable<T> query = _dbset.AsNoTracking();

            if (filter is not null)
            {
                query = query.Where(filter);
            }

            // Get total count before paging
            var totalCount = await query.CountAsync();

            // Apply paging
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Create and return paged result
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
            //if (entity is ISoftDeletable softDeletableEntity)
            //{
            //    // 2. Nếu là VIP, chỉ "đánh dấu" đã rời đi
            //    softDeletableEntity.IsActive = false;
            //    _dbSet.Update(entity); // Báo cho EF Core biết là có sự thay đổi (UPDATE)
            //}
            //else
            //{
                _dbset.Remove(entity);
            //}
        }

        public void Update(T entity)
        {
            _dbset.Update(entity);
        }
    }
}
