
using FundingAccount.API.Application.Services.v1.FundingServices;
using FundingAccount.API.Application.Services.v1.UtilitiesServices;
using FundingAccount.API.Infrastructure.ExceptionMiddleware;
using Microsoft.AspNetCore.Mvc.Versioning;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
      .ConfigureApiBehaviorOptions(options =>
      {
          //options.InvalidModelStateResponseFactory = context =>
          //{
          //    var errors = context.ModelState.Values
          //        .SelectMany(v => v.Errors)
          //        .Select(e => JsonSerializer.Deserialize<ErrorMessageFormat>(e.ErrorMessage));

          //    return new BadRequestObjectResult(errors);
          //};
      });


//builder.Services.AddFluentValidationAutoValidation();

//builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters().AddValidatorsFromAssemblies(IEnumerable<Assembly.GetExecutingAssembly()>);


//builder.Services.AddFluentValidation(config =>
//{
//    config.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
//});

//builder.Services.AddTransient<IValidatorInterceptor, CustomValidationInterceptor>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ConnectionMultiplexer>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<AppSettings>>().Value;
    var configuration = ConfigurationOptions.Parse(settings.RedisConnectionString, true);

    return ConnectionMultiplexer.Connect(configuration);
});


builder.Services.AddHttpClient<IRegisteredFundingAccountService, Registered_FundingAccountService>();
builder.Services.AddScoped<IRegisteredFundingAccountService, Registered_FundingAccountService>();

builder.Services.AddHttpClient<IUnregisteredFundingAccountService, Unregistered_FundingAccountService>();
builder.Services.AddScoped<IUnregisteredFundingAccountService, Unregistered_FundingAccountService>();

builder.Services.AddHttpClient<ICommonFundingAccountService, Common_FundingAccountService>();
builder.Services.AddScoped<ICommonFundingAccountService, Common_FundingAccountService>();

builder.Services.AddHttpClient<IUtilityService, UtilitiesService>();
builder.Services.AddScoped<IUtilityService, UtilitiesService>();


//By connecting here we are making sure that our service
//cannot start until redis is ready. This might slow down startup,
//but given that there is a delay on resolving the ip address
//and then creating the connection it seems reasonable to move
//that cost to startup instead of having the first request pay the
//penalty.

//builder.Services.AddCustomHealthCheck(Configuration);

//........................................Key Vault...........................................................................
// For accessing secret (DB connection string) from azure key vault using system assigned managed identity

// Install-package Azure.Security.KeyVault.Secrets
// For using user assigned managed identity, pass this UAManagedIdentityClientId in DefaultAzureCredential()
//string UAManagedIdentityClientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
//string UAManagedIdentityClientId = "75c47897-df76-40c4-b322-0e7a0216b203";
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
// For Azure App Config (using app service and system assigned managed identity)
// This code is by using service principle whose creadentials are stored in launchsettings.json
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


builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

Log.Logger = new LoggerConfiguration()
.MinimumLevel.Information()
.WriteTo.Console()
.CreateLogger();
builder.Logging.AddSerilog();

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
