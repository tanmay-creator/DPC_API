using StackExchange.Redis;

namespace Utilities.API.HelperMethods.RedisCache
{
    public interface IConnectionHelper
    {
        IConnectionMultiplexer RedisConnection { get; } 
    }
}
