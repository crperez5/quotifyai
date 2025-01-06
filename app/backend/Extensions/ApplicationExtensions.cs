namespace MinimalApi.Extensions;

internal static class ApplicationExtensions
{
    internal static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IAzureBlobStorageService, AzureBlobStorageService>();
        return services;
    }
}