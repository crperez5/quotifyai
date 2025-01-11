namespace MinimalApi.HealthChecks;

internal class CosmosDbHealthCheck(AppDbContext appDbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await appDbContext.Conversations.CountAsync(cancellationToken);
            return HealthCheckResult.Healthy("Cosmos DB is reachable and operational.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Cosmos DB is not reachable.", ex);
        }
    }
}