using Azure.Identity;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Transaction.API;
using Transaction.API.Application.Services.v1.PaymentsServices;
using Transaction.API.Application.Services.v1.Services.Abstraction;
using Transaction.API.Application.Services.v1.UtilitiesServices;
using Transaction.API.Infrastructure.ExceptionMiddleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IUtilitiesService, UtilitiesService>();
builder.Services.AddScoped<IUtilitiesService, UtilitiesService>();

builder.Services.AddHttpClient<IRegisteredPaymentService, RegisteredPaymentService>();
builder.Services.AddScoped<IRegisteredPaymentService, RegisteredPaymentService>();

builder.Services.AddHttpClient<IUnregisteredPaymentService, UnregisteredPaymentService>();
builder.Services.AddScoped<IUnregisteredPaymentService, UnregisteredPaymentService>();


// Install-package Azure.Security.KeyVault.Secrets
var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");
var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
var clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");
//var kvUri = "https://dpcrefapp-keyvault.vault.azure.net/";

//SecretClientOptions options = new SecretClientOptions()
//{
//    Retry =
//        {
//            Delay= TimeSpan.FromSeconds(2),
//            MaxDelay = TimeSpan.FromSeconds(16),
//            MaxRetries = 5,
//            Mode = RetryMode.Exponential
//         }
//};
//var client = new SecretClient(new Uri(kvUri), /*new ClientSecretCredential(tenantId, clientId, clientSecret)*/ new DefaultAzureCredential());
////SqlDBContainerConnString, SqlDBLocalConnString
//KeyVaultSecret DbSecret = client.GetSecret("SqlDBContainerConnString");
//string DbConnectionString = DbSecret.Value;
//Console.WriteLine("\n**********Secret from azure key vault is:-  " + DbConnectionString + "\n");



//........................................Azure App Configuration.................................................................
// Packages: Microsoft.Extensions.Configuration.AzureAppConfiguration, Azure.Identity
builder.Configuration.AddAzureAppConfiguration(o =>
{
    // Running on app service: endpoint
    o.Connect(new Uri("https://dpcrefapp-appconfiguration.azconfig.io"), new DefaultAzureCredential())
    .Select("ACICredentials:*", LabelFilter.Null)
    // Configure to reload configuration if the registered sentinel key is modified
    .ConfigureRefresh(refreshOptions => refreshOptions.Register("UserProfile:Settings:Sentinel", refreshAll: true).SetCacheExpiration(TimeSpan.FromSeconds(10))); ;


    // Running locally : connection string 
    //o.Connect("Endpoint=https://dpcrefapp-appconfiguration.azconfig.io;Id=rCDh;Secret=62nHYUf5j/hWsvaC5lJLh17cAgb2E1G1v06VsOaK5ko=")
    //.Select("ACICredentials:*", LabelFilter.Null)
    //// Configure to reload configuration if the registered sentinel key is modified
    //.ConfigureRefresh(refreshOptions => refreshOptions.Register("UserProfile:Settings:Sentinel", refreshAll: true).SetCacheExpiration(TimeSpan.FromSeconds(10)));
});

// Add Azure App Configuration middleware to the container of services.
builder.Services.AddAzureAppConfiguration();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("ACICredentials"));
builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
    config.ApiVersionReader = new UrlSegmentApiVersionReader();
});

Log.Logger = new LoggerConfiguration()
.MinimumLevel.Information()
.WriteTo.Console()
.CreateLogger();
builder.Logging.AddSerilog();

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

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
