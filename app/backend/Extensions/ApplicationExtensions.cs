using Microsoft.Azure.Cosmos;

namespace MinimalApi.Extensions;

internal static class ApplicationExtensions
{
    internal static async Task AddApplicationServicesAsync(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAzureBlobStorageService, AzureBlobStorageService>();

        await services.AddCosmosDbAsync(configuration);
    }

    private static async Task AddCosmosDbAsync(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["CosmosDbConnectionString"]!;
        var cosmosDbDatabaseName = Environment.GetEnvironmentVariable("CosmosDbDatabaseName") ?? throw new InvalidOperationException("Cosmos DB database name is not set.");
        var cosmosDbTableName = Environment.GetEnvironmentVariable("CosmosDbTableName") ?? throw new InvalidOperationException("Cosmos DB table name is not set.");
        var cosmosDbPartitionKey = Environment.GetEnvironmentVariable("CosmosDbPartitionKey") ?? throw new InvalidOperationException("Cosmos DB partition key is not set.");

        var client = new CosmosClient(connectionString);
        Database database = await client.CreateDatabaseIfNotExistsAsync(cosmosDbDatabaseName);
        Container container = await database.CreateContainerIfNotExistsAsync(id: cosmosDbTableName, partitionKeyPath: cosmosDbPartitionKey);

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseCosmos(connectionString, cosmosDbDatabaseName);
        });
    }
}