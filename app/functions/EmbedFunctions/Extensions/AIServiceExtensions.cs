namespace EmbedFunctions.Extensions;

internal static class AIServiceExtensions
{
    internal static IServiceCollection AddAIServices(this IServiceCollection services, IConfiguration configuration)
    {
        var openAIEmbeddingsModelId = Environment.GetEnvironmentVariable("OpenAIEmbeddingsModelId") ?? throw new InvalidOperationException("OpenAI embeddings model ID is not.");
        var openAIApiKey = configuration["OpenAIApiKey"] ?? throw new InvalidOperationException("OpenAI API key is not.");
        var openAIOrgId = configuration["OpenAIOrgId"] ?? throw new InvalidOperationException("OpenAI Org ID is not.");

        // Register the kernel with the dependency injection container
        // and add Text Embedding Generation service.

        var kernelBuilder = services.AddKernel();
        kernelBuilder.Services.AddLogging(configure => configure.AddConsole());
        kernelBuilder.Services.AddLogging(configure => configure.SetMinimumLevel(LogLevel.Trace));

        kernelBuilder.AddOpenAITextEmbeddingGeneration(
            openAIEmbeddingsModelId,
            openAIApiKey,
            openAIOrgId);

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