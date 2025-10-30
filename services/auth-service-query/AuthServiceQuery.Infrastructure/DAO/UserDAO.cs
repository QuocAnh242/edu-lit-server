﻿using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.DAO.Interfaces;
using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.DAO
{
    public class UserDAO : IUserDAO
    {
        private readonly IUnitOfWork _uow;
        private readonly AuthDbContext _db;

        public UserDAO(IUnitOfWork uow)
        {
            _uow = uow;
            _db = (AuthDbContext)_uow.Context;
        }

        public async Task<User> GetByIdAsync(Guid id) => await _db.Set<User>().FindAsync(id);

        public async Task<List<User>> GetAllAsync() => await _db.Set<User>().ToListAsync();

        public async Task<(List<User> Items, int TotalCount)> GetPagedAsync(int page, int size)
        {
            if (page < 1) page = 1;
            if (size < 1) size = 20;

            var query = _db.Set<User>()
                .Include(u => u.Role)
                .AsNoTracking()
                .OrderByDescending(u => u.CreatedAt ?? DateTime.UnixEpoch);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task AddAsync(User user)
        {
            _db.Set<User>().Add(user);
            await _uow.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _db.Set<User>().Update(user);
            await _uow.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                _db.Set<User>().Remove(user);
                await _uow.SaveChangesAsync();
            }
        }
    }
}
