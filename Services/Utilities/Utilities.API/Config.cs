namespace Utilities.API
{
    public class LogLevel
    {
        public string Default { get; set; }
        public string MicrosoftAspNetCore { get; set; }
    }
    public class Logging
    {
        public LogLevel LogLevel { get; set; }
    }
    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
    }
    public class Config
    {
        public Logging Logging { get; set; }
        public string AllowedHosts { get; set; }
        public string RedisURL { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }

    }
}
