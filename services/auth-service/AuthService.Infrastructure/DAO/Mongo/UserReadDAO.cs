using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Infrastructure.DAO.Interfaces;
using MongoDB.Driver;
using AuthService.Domain.Entities.ReadModels;

namespace AuthService.Infrastructure.DAO.Mongo
{
    public sealed class UserReadDAO : IUserReadDAO
    {
        private readonly IMongoCollection<UserReadModel> _col;

        public UserReadDAO(IMongoCollection<UserReadModel> collection)
        {
            _col = collection;
            var keys = Builders<UserReadModel>.IndexKeys.Ascending(x => x.Username);
            _col.Indexes.CreateOne(new CreateIndexModel<UserReadModel>(keys));
        }

        public async Task<UserReadModel?> GetByIdAsync(Guid id)
            => await _col.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<List<UserReadModel>> GetAllAsync()
            => await _col.Find(FilterDefinition<UserReadModel>.Empty).ToListAsync();

        public async Task<(List<UserReadModel> Items, int TotalCount)> GetPagedAsync(int page, int size)
        {
            page = page < 1 ? 1 : page;
            size = size < 1 ? 20 : size;

            var filter = FilterDefinition<UserReadModel>.Empty;

            var total = (int)await _col.CountDocumentsAsync(filter);
            var items = await _col.Find(filter)
                .SortByDescending(x => x.CreatedAt)
                .Skip((page - 1) * size)
                .Limit(size)
                .ToListAsync();

            return (items, total);
        }

        public async Task UpsertAsync(UserReadModel model)
        {
            var filter = Builders<UserReadModel>.Filter.Eq(x => x.Id, model.Id);
            await _col.ReplaceOneAsync(filter, model, new ReplaceOptions { IsUpsert = true });
        }

        public async Task DeleteAsync(Guid id)
        {
            await _col.DeleteOneAsync(x => x.Id == id);
        }
    }
}