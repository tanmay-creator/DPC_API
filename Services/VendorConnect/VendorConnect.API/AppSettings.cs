namespace VendorConnect.API;

public class AppSettings
{
    public Connectionstrings ConnectionStrings { get; set; }

    public string RedisConnectionString { get; set; }
    public string FundingAccountUrl { get; set; }

    public string FundingAccountBaseUrl { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string XAuthKey { get; set; }
    public string AuthTokenUrl { get; set; }
    public string APIM_SubscriptionKey { get; set;}
    public string ApimGetTokenUrl { get; set; }
    public string UserProfileBaseUrl { get; set; }
    public string UtilityServiceBaseUrl { get; set; }
    public string ACIPaymentUrl { get; set; }

}

public class Connectionstrings
{
    public string DefaultConnection { get; set; }
}

public class Logging
{
    public bool IncludeScopes { get; set; }
    public Loglevel LogLevel { get; set; }
}

public class Loglevel
{
    public string Default { get; set; }
    public string System { get; set; }
    public string Microsoft { get; set; }
}
