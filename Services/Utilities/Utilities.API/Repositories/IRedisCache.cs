using StackExchange.Redis;

namespace RedisService.API.Repositories
{
    public interface IRedisCache
    {
        List<T> SetListToRedis<T>(string key, List<T> list);
        Task<T> GetCacheDataAsync<T>(string key);
        T GetCacheData<T>(string key);
        string GetCacheData(string key);
        Task<RedisValue> GetCacheDataAsync(string key);
        Task<bool> SetCacheDataAsync(string key, string value, DateTimeOffset expirationTime);

        Task<bool> SetCacheDataHashAsync<T>(string customerId, int key, T value);
        Task<bool> SetCacheDataSetAsync<T>(string customerId, int key, T value);
        object RemoveData(string key);
        Task<bool> SetCacheDataAsync<T>(string key, List<string> value, DateTimeOffset expirationTime);

        Task<bool> isKeyExist(string key);

    }
}
