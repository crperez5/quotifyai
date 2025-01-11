namespace MinimalApi.Extensions;

internal static class ApplicationExtensions
{
    internal static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAzureBlobStorageService, AzureBlobStorageService>();

        services.AddCosmosDb(configuration);

        return services;
    }

    private static IServiceCollection AddCosmosDb(this IServiceCollection services, IConfiguration configuration)
    {
        var cosmosDbDatabaseName = Environment.GetEnvironmentVariable("CosmosDbDatabaseName") ?? throw new InvalidOperationException("Cosmos DB database name is not set.");

        var azureKeyVaultEndpoint = Environment.GetEnvironmentVariable("AZURE_KEY_VAULT_ENDPOINT") ?? throw new InvalidOperationException("Azure Key Vault endpoint is not set.");

        // var secretClient = new SecretClient(new Uri(azureKeyVaultEndpoint), new DefaultAzureCredential());
        var connectionString = configuration["CosmosDbConnectionString"]!;
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseCosmos(connectionString, cosmosDbDatabaseName);
        });

        return services;
    }
}