using StackExchange.Redis;

namespace likhitan.Common.Services
{
    public class RedisService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _db = _redis.GetDatabase();
        }

        public async Task SetValueAsync(string key, string value, TimeSpan? expiry = null)
        {
            await _db.StringSetAsync(key, value, expiry);
        }

        public async Task<string?> GetValueAsync(string key)
        {
            var value = await _db.StringGetAsync(key);
            return value.HasValue ? value.ToString() : null;
        }

        public async Task<bool> DeleteKeyAsync(string key)
        {
            return await _db.KeyDeleteAsync(key);
        }
    }
}
