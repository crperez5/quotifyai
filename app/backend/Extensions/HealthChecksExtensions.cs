namespace MinimalApi.Extensions;

internal static class HealthChecksExtensions
{
    public static void ConfigureHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<VectorStoreHealthCheck>("Vector Store Health Check", failureStatus: HealthStatus.Unhealthy)
            .AddCheck<CosmosDbHealthCheck>("Cosmos DB Health Check", failureStatus: HealthStatus.Unhealthy);
    }
}