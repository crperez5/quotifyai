namespace EmbedFunctions.Extensions;

internal static class AIServiceExtensions
{
    internal static IServiceCollection AddAIServices(this IServiceCollection services)
    {
        var azureAIEndpoint = Environment.GetEnvironmentVariable("AZURE_AI_ENDPOINT") ?? throw new InvalidOperationException("Azure AI endpoint is not set.");
        var embeddingsDeploymentName = Environment.GetEnvironmentVariable("EmbeddingsDeploymentName") ?? throw new InvalidOperationException("Embeddings deployment name is not set.");

        // Register the kernel with the dependency injection container
        // and add Text Embedding Generation service.

        var kernelBuilder = services.AddKernel();
        kernelBuilder.Services.AddLogging(configure => configure.AddConsole());
        kernelBuilder.Services.AddLogging(configure => configure.SetMinimumLevel(LogLevel.Trace));

        kernelBuilder.AddAzureOpenAITextEmbeddingGeneration(
            embeddingsDeploymentName,
            azureAIEndpoint,
            new DefaultAzureCredential());

        AddVectorStore(services, kernelBuilder);
        return services;
    }

    private static void AddVectorStore(IServiceCollection services, IKernelBuilder kernelBuilder)
    {
        var vectorStoreEndpoint = Environment.GetEnvironmentVariable("VectorStoreEndpoint") ?? throw new InvalidOperationException("Vector Store endpoint is not set.");
        var vectorStoreUseHttps = Environment.GetEnvironmentVariable("VectorStoreUseHttps") ?? throw new InvalidOperationException("Vector Store useHttps is not set.");
        var vectorStorePort = Environment.GetEnvironmentVariable("VectorStorePort") ?? throw new InvalidOperationException("Vector Store port is not set.");

        var vectorStoreEndpointUri = new Uri(vectorStoreEndpoint);
        int port = int.Parse(vectorStorePort);
        var useHttps = bool.Parse(vectorStoreUseHttps);

        // Add the configured vector store record collection type to the
        // dependency injection container.

        kernelBuilder.AddQdrantVectorStoreRecordCollection<Guid, TextSnippet<Guid>>(
            "instructions",
            vectorStoreEndpointUri.Host,
            port,
            useHttps,
            apiKey: "");

        services.AddSingleton(new UniqueKeyGenerator<Guid>(Guid.NewGuid));
        services.AddSingleton(new UniqueKeyGenerator<string>(() => Guid.NewGuid().ToString()));            

        services.AddSingleton(sp => new QdrantClient(vectorStoreEndpointUri.Host, port, useHttps));
        services.AddQdrantVectorStore();
    }
}