using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;
using LessonService.Application.IServices;

namespace LessonService.Infrastructure.Persistance.DistributedCaches
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly JsonSerializerOptions _jsonOptions;

        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
            _db = _redis.GetDatabase();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(value, _jsonOptions);
            await _db.StringSetAsync(key, json, expiry).ConfigureAwait(false);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var rv = await _db.StringGetAsync(key).ConfigureAwait(false);
            if (!rv.HasValue) return default;
            return JsonSerializer.Deserialize<T>(rv, _jsonOptions);
        }

        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key).ConfigureAwait(false);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await _db.KeyExistsAsync(key).ConfigureAwait(false);
        }

        public async Task AddToSortedSetAsync(string key, string member, double score)
        {
            await _db.SortedSetAddAsync(key, member, score).ConfigureAwait(false);
        }

        public async Task<IEnumerable<string>> GetRangeFromSortedSetAsync(string key, long start, long stop, bool descending = true)
        {
            var order = descending ? Order.Descending : Order.Ascending;
            var values = await _db.SortedSetRangeByRankAsync(key, start, stop, order).ConfigureAwait(false);
            return values.Select(v => (string)v).ToList();
        }

        public async Task<long> GetSortedSetLengthAsync(string key)
        {
            return await _db.SortedSetLengthAsync(key).ConfigureAwait(false);
        }

        public async Task RemoveFromSortedSetAsync(string key, string member)
        {
            await _db.SortedSetRemoveAsync(key, member).ConfigureAwait(false);
        }

        public async Task ClearSortedSetAsync(string key)
        {
            // remove entire key
            await _db.KeyDeleteAsync(key).ConfigureAwait(false);
        }

        public async Task PublishAsync(string channel, string message)
        {
            var sub = _redis.GetSubscriber();
            await sub.PublishAsync(channel, message).ConfigureAwait(false);
        }

        public void Subscribe(string channel, Action<string> handler)
        {
            var sub = _redis.GetSubscriber();
            sub.Subscribe(channel, (redisChannel, value) =>
            {
                handler(value.HasValue ? value.ToString()! : string.Empty);
            });
        }
    }
}
