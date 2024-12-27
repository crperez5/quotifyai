using Qdrant.Client;

namespace MinimalApi.HealthChecks;

internal class VectorStoreHealthCheck(QdrantClient client) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var collectionExists = await client.CollectionExistsAsync("test", cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Vector Store endpoint is unhealthy: {ex.Message}");
        }
    }
}