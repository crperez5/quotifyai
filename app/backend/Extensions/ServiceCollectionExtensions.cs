namespace MinimalApi.Extensions;

internal static class ServiceCollectionExtensions
{
    private static readonly DefaultAzureCredential s_azureCredential = new();

    internal static IServiceCollection AddAzureServices(this IServiceCollection services)
    {
        services.AddSingleton<BlobServiceClient>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var azureStorageAccountConnectionString = config["AzureStorageAccountConnectionString"] ?? string.Empty;
            if (!string.IsNullOrEmpty(azureStorageAccountConnectionString))
            {
                return new BlobServiceClient(azureStorageAccountConnectionString);
            }

            var azureStorageAccountEndpoint = config["AzureStorageAccountEndpoint"];
            ArgumentNullException.ThrowIfNullOrEmpty(azureStorageAccountEndpoint);

            return new BlobServiceClient(new Uri(azureStorageAccountEndpoint), s_azureCredential);
        });

        services.AddSingleton<BlobContainerClient>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var azureStorageContainer = config["AzureStorageContainer"];
            return sp.GetRequiredService<BlobServiceClient>().GetBlobContainerClient(azureStorageContainer);
        });

        services.AddSingleton<AzureBlobStorageService>();

        return services;
    }

    internal static IServiceCollection AddCustomSignalR(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddSingleton<MessageProcessingService>();
        services.AddHostedService(sp => sp.GetRequiredService<MessageProcessingService>());
        return services;
    }
}