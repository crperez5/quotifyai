using MinimalApi.HealthChecks;

namespace MinimalApi.Extensions;

internal static class HealthChecksExtensions
{
    public static void ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddCheck<VectorStoreHealthCheck>("Vector Store Health Check", failureStatus: HealthStatus.Unhealthy);
    }
}