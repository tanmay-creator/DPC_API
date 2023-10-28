using StackExchange.Redis;
using Utilities.API.HelperMethods.RedisCache;

namespace RedisService.API.Helper
{
    public class ConnectionHelper:IConnectionHelper
    {
        //private static IConfiguration _configuration;
        private readonly Lazy<ConnectionMultiplexer> LazyConnection;

        public ConnectionHelper(IConfiguration configuration)
        {
            var configurationOptions = new ConfigurationOptions
            {
                EndPoints = { configuration["ConnectionStrings--RedisConnectionString"] }
            };

            LazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(configurationOptions));
        }
        public IConnectionMultiplexer RedisConnection => LazyConnection.Value;
        public IDatabase RedisCache => RedisConnection.GetDatabase();


        #region Old working code
        static ConnectionHelper()
        {
            ConnectionHelper.lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(ConfigurationManager.AppSetting["RedisURL"]);
            });

        }
        private static Lazy<ConnectionMultiplexer> lazyConnection;
        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
        #endregion
        #region Old R& D Code
        //ConnectionHelper(IConfiguration configuration)
        //{
        //    //_configuration = ConfigurationManager.AppSetting.GetValue()

        //    ConnectionHelper.lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        //    {
        //        //return ConnectionMultiplexer.Connect(ConfigurationManager.AppSetting["RedisURL"]);
        //        return ConnectionMultiplexer.Connect(_configuration["RedisURL"]);
        //    });

        //}
        //private static Lazy<ConnectionMultiplexer> lazyConnection;
        //public static ConnectionMultiplexer Connection
        //{
        //    get
        //    {
        //        return lazyConnection.Value;
        //    }
        //}
        //private static IConfiguration _configuration;
        //public ConnectionHelper(IConfiguration configuration) 
        //{ 
        //    _configuration = configuration;
        //}
        //private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        //{

        //     string redisCacheConnection = _configuration["RedisCacheSecretKey"];
        //     return ConnectionMultiplexer.Connect(redisCacheConnection);
        //});

        //public static ConnectionMultiplexer Connection
        //{
        //    get
        //    {
        //        return lazyConnection.Value;
        //    }
        //}
        #endregion

    }
}
