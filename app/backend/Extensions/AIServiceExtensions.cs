using Microsoft.SemanticKernel;
using Qdrant.Client;

namespace MinimalApi.Extensions;

internal static class AIServiceExtensions
{
    internal static IServiceCollection AddAIServices(this IServiceCollection services)
    {
        var kernelBuilder = services.AddKernel();
        kernelBuilder.Services.AddLogging(configure => configure.AddConsole());
        kernelBuilder.Services.AddLogging(configure => configure.SetMinimumLevel(LogLevel.Trace));

        AddVectorStore(services);
        return services;
    }

    private static void AddVectorStore(IServiceCollection services)
    {
        var vectorStoreEndpoint = Environment.GetEnvironmentVariable("VectorStoreEndpoint") ?? throw new InvalidOperationException("Vector Store endpoint is not set.");
        var vectorStoreUseHttps = Environment.GetEnvironmentVariable("VectorStoreUseHttps") ?? throw new InvalidOperationException("Vector Store useHttps is not set.");
        var vectorStorePort = Environment.GetEnvironmentVariable("VectorStorePort") ?? throw new InvalidOperationException("Vector Store port is not set.");

        int port = int.Parse(vectorStorePort);
        var useHttps = bool.Parse(vectorStoreUseHttps);

        services.AddSingleton<QdrantClient>(sp => new QdrantClient(vectorStoreEndpoint, port, useHttps));
        services.AddQdrantVectorStore();
    }
}