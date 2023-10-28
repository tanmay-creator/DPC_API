using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Utilities.API;
using Utilities.API.ExceptionMiddleware;
using Serilog;
using ServiceStack;
using Utilities.API.HelperMethods.RedisCache;
using RedisService.API.Helper;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

var builder = WebApplication.CreateBuilder(args);
var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");
var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
var clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");
var keyVaultUrl = Environment.GetEnvironmentVariable("KEY_VAULT_URL");
var appConfigUrl = Environment.GetEnvironmentVariable("APP_CONFIGURATION_URL");
// Add services to the container.

builder.Services.AddControllers()
      .AddJsonOptions(options =>
      {
          options.JsonSerializerOptions.PropertyNamingPolicy = null;
          options.JsonSerializerOptions.WriteIndented = true;
      });

#region Azure Key Vault
//----------------------------------Key Vault------------------------------------------------------------------------------------
// Install-package Azure.Security.KeyVault.Secrets

var client = new SecretClient(new Uri(keyVaultUrl), /*new ClientSecretCredential(tenantId, clientId, clientSecret)*/ new DefaultAzureCredential());
KeyVaultSecret DbSecret = client.GetSecret("ConnectionStrings--SqlDBConnectionString");
KeyVaultSecret RedisSecret = client.GetSecret("ConnectionStrings--RedisConnectionString");
string SqlConnectionString = DbSecret.Value;
string RedisConnectionString = RedisSecret.Value;
Console.WriteLine("Connection strings are : \n SqlDBConnectionString=> "+SqlConnectionString+"\n RedisConnectionString=> "+RedisConnectionString+"\n");

#region Key vault by reloading appsettings.json
var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build()
            .Get<Config>();
config.RedisURL = RedisConnectionString;

var jsonWriteOptions = new JsonSerializerOptions()
{
    WriteIndented = true
};
jsonWriteOptions.Converters.Add(new JsonStringEnumConverter());

var newJson = System.Text.Json.JsonSerializer.Serialize(config, jsonWriteOptions);
Console.WriteLine(newJson);
var appSettingsPath = Path.GetFullPath("appsettings.json");
File.WriteAllText(appSettingsPath, newJson);
#endregion

#region Keyvault R&D
builder.Services.AddStackExchangeRedisCache(option =>
{
    option.Configuration = builder.Configuration.GetConnectionString
        (RedisConnectionString);
    //option.InstanceName = "master";
});

//builder.Services.AddStackExchangeRedisExtensions<RedisCache>(options => { options.Configuration = configuration[RedisConnectionString]; });
//var _redisConnection = await RedisConnectionString.InitializeAsync(connectionString: ConfigurationManager.AppSettings["CacheConnection"].ToString());
#endregion

//builder.Services.AddScoped<IConnectionHelper, ConnectionHelper>();
//builder.Services.AddScoped<IConnectionMultiplexer, RedisCache>()

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
                 ConnectionMultiplexer.Connect(RedisConnectionString));

#endregion


#region Azure App Configuration
//........................................Azure App Configuration.................................................................
// Packages: Microsoft.Extensions.Configuration.AzureAppConfiguration, Azure.Identity
builder.Configuration.AddAzureAppConfiguration(o =>
{
    // Running with Service Principle: endpoint
    o.Connect(new Uri(appConfigUrl), new DefaultAzureCredential())
    .Select("Utility:*", LabelFilter.Null)
    // Configure to reload configuration if the registered sentinel key is modified
    .ConfigureRefresh(refreshOptions => refreshOptions.Register("UserProfile:Settings:Sentinel", refreshAll: true).SetCacheExpiration(TimeSpan.FromSeconds(10))); ;


    // Running locally : connection string 
    //o.Connect("Endpoint=https://dpcrefapp-appconfiguration.azconfig.io;Id=rCDh;Secret=62nHYUf5j/hWsvaC5lJLh17cAgb2E1G1v06VsOaK5ko=")
    //.Select("ACICredentials:*", LabelFilter.Null)
    //// Configure to reload configuration if the registered sentinel key is modified
    //.ConfigureRefresh(refreshOptions => refreshOptions.Register("UserProfile:Settings:Sentinel", refreshAll: true).SetCacheExpiration(TimeSpan.FromSeconds(10)));
});

builder.Services.AddAzureAppConfiguration();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("Utility"));
#endregion

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IRedisCache, RedisCache>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<ApplicationDbContext>
        //(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
          (options => options.UseSqlServer(builder.Configuration.GetConnectionString(SqlConnectionString)));

Log.Logger = new LoggerConfiguration()
.MinimumLevel.Information()
.WriteTo.Console()
.CreateLogger();
builder.Logging.AddSerilog();

builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
    config.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

