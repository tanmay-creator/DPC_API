using Newtonsoft.Json;
using RedisService.API.Helper;
using StackExchange.Redis;
using System.Reflection;

namespace RedisService.API.Repositories
{
    public class RedisCache : IRedisCache
    {
        private IDatabase _db;
        //private IRedisCacheClient _redisCacheClient;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        public RedisCache(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
            ConfigureRedis();
        }

        private void ConfigureRedis()
        {
            //_db = ConnectionHelper.Connection.GetDatabase();
            _db = _connectionMultiplexer.GetDatabase();
        }

        public T GetCacheData<T>(string key)
        {
            var value = _db.StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default;
        }

        public object RemoveData(string key)
        {
            bool _isKeyExist = _db.KeyExists(key);
            if (_isKeyExist == true)
            {
                return _db.KeyDelete(key);
            }
            return false;
        }

        public bool SetCacheData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            var isSet = _db.StringSet(key, JsonConvert.SerializeObject(value), expiryTime);
            return isSet;
        }

        public bool SetCacheData<T>(string key, List<string> value, DateTimeOffset expirationTime)
        {
            TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            var isSet = _db.StringSet(key, JsonConvert.SerializeObject(value), expiryTime);
            return isSet;
        }
        public bool SetCacheDataHash<T>(string customerId,int key, T value)
        {
            //TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
             var isSet = _db.HashSet(customerId, key.ToString(),JsonConvert.SerializeObject(value));
            return isSet;
        }


        public bool SetCacheDataSet<T>(string customerId, int key, T value)
        {
            //TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            var x = _db.SetAdd(customerId, key.ToString());
            HashEntry[] hashEntries = ToHashEntries(value);
            _db.HashSet(key.ToString(), hashEntries);
             return true;
        }

        public static HashEntry[] ToHashEntries(object obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            return properties
                .Where(x => x.GetValue(obj) != null) // <-- PREVENT NullReferenceException
                .Select
                (
                      property =>
                      {
                          object propertyValue = property.GetValue(obj);
                          string hashValue;

                          // This will detect if given property value is 
                          // enumerable, which is a good reason to serialize it
                          // as JSON!
                          if (propertyValue is IEnumerable<object>)
                          {
                              // So you use JSON.NET to serialize the property
                              // value as JSON
                              hashValue = JsonConvert.SerializeObject(propertyValue);
                          }
                          else
                          {
                              hashValue = propertyValue.ToString();
                          }

                          return new HashEntry(property.Name, hashValue);
                      }
                )
                .ToArray();
        }

        //public string GetCacheDataAsync(string key)
        //{
        //    var value = _db.StringGet(key);
        //    if (!string.IsNullOrEmpty(value))
        //    {
        //        return value;
        //    }
        //    return default;
        //}

       //public Task<T> GetCacheDataAsync<T>(string key)
       // {
       //     var value = _db.StringGet(key);
       //     if (!string.IsNullOrEmpty(value))
       //     {
       //         return JsonConvert.DeserializeObject<T>(value);
       //     }
       //     return default;
       // }

        public async Task<RedisValue> GetCacheDataAsync(string key)
        {
            var value = await _db.StringGetAsync(key);
            
            return value;
        }

        public async Task<bool> SetCacheDataAsync(string key, string value, DateTimeOffset expirationTime)
        {
            TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            var isSet = await _db.StringSetAsync(key, value, expiryTime);
            return isSet;
        }

        public Task<bool> SetCacheDataHashAsync<T>(string customerId, int key, T value)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> SetCacheDataSetAsync<T>(string customerId, int key, T value)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> SetCacheDataAsync<T>(string key, List<string> value, DateTimeOffset expirationTime)
        {
            TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            var isSet = await _db.StringSetAsync(key, JsonConvert.SerializeObject(value), expiryTime);
            return isSet;
        }

        public Task<T> GetCacheDataAsync<T>(string key)
        {
            throw new System.NotImplementedException();
        }
        
        public string GetCacheData(string key)
        {
            var value = _db.StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }
            return default;
        }

        public async Task<bool> isKeyExist(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }
            else if (_db.KeyExists(key))
            {
                return true;
            }
            return false;        }

        public List<T> SetListToRedis<T>(string key, List<T> list)
        {
            throw new System.NotImplementedException();
        }

        //public List<T> SetListToRedis<T>(string key, List<T> list)
        //{
        //    _db.Add;
        //}
    }
}
