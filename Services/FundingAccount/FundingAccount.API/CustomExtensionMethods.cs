using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FundingAccount.API;

public static class CustomExtensionMethods
{
    public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());
        
        hcBuilder
            .AddRedis(
                configuration["ConnectionString"],
                name: "redis-check",
                tags: new string[] { "redis" });        

        return services;
    }
}
