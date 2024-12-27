using MinimalApi.HealthChecks;

namespace MinimalApi.Extensions;

internal static class HealthChecksExtensions
{
    public static void ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddCheck<VectorStoreHealthCheck>("Vector Store Health Check", failureStatus: HealthStatus.Unhealthy);
            
        services.AddHealthChecksUI(opt =>
        {
            opt.SetEvaluationTimeInSeconds(30);    
            opt.MaximumHistoryEntriesPerEndpoint(60);
            opt.SetApiMaxActiveRequests(1);
            opt.AddHealthCheckEndpoint("feedback api", "/health");
        });
    }
}