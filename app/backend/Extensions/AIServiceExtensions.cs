namespace MinimalApi.Extensions;

internal static class AIServiceExtensions
{
    internal static IServiceCollection AddAIServices(this IServiceCollection services, IConfiguration configuration)
    {
        var azureAIEndpoint = Environment.GetEnvironmentVariable("AZURE_AI_ENDPOINT") ?? throw new InvalidOperationException("Azure AI endpoint is not set.");
        var embeddingsDeploymentName = Environment.GetEnvironmentVariable("EmbeddingsDeploymentName") ?? throw new InvalidOperationException("Embeddings deployment name is not set.");

        var openAIModelId = Environment.GetEnvironmentVariable("OpenAIModelId") ?? throw new InvalidOperationException("OpenAI model ID is not.");
        var openAIApiKey = configuration["OpenAIApiKey"] ?? throw new InvalidOperationException("OpenAI API key is not.");        
        var openAIOrgId = configuration["OpenAIOrgId"] ?? throw new InvalidOperationException("OpenAI Org ID is not.");                

        // Register the kernel with the dependency injection container
        // and add Chat Completion and Text Embedding Generation services.

        var kernelBuilder = services.AddKernel();
        kernelBuilder.Services.AddLogging(configure => configure.AddConsole());
        kernelBuilder.Services.AddLogging(configure => configure.SetMinimumLevel(LogLevel.Trace));

        kernelBuilder.AddOpenAIChatCompletion(
            modelId: openAIModelId,
            apiKey: openAIApiKey,
            orgId: openAIOrgId);

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

        int port = int.Parse(vectorStorePort);
        var useHttps = bool.Parse(vectorStoreUseHttps);

        // Add the configured vector store record collection type to the
        // dependency injection container.

        kernelBuilder.AddQdrantVectorStoreRecordCollection<Guid, TextSnippet<Guid>>(
            "instructions",
            vectorStoreEndpoint,
            port,
            useHttps,
            apiKey: "");

        // Add a text search implementation that uses the registered vector store record collection for search.

        kernelBuilder.AddVectorStoreTextSearch<TextSnippet<Guid>>(
            new TextSearchStringMapper((result) => (result as TextSnippet<Guid>)!.Text!),
            new TextSearchResultMapper((result) =>
            {
                // Create a mapping from the Vector Store data type to the data type returned by the Text Search.
                // This text search will ultimately be used in a plugin and this TextSearchResult will be returned to the prompt template
                // when the plugin is invoked from the prompt template.
                var castResult = result as TextSnippet<Guid>;
                return new TextSearchResult(value: castResult!.Text!) { Name = castResult.ReferenceDescription, Link = castResult.ReferenceLink };
            }));

        services.AddSingleton(new UniqueKeyGenerator<Guid>(Guid.NewGuid));
        services.AddSingleton(new UniqueKeyGenerator<string>(() => Guid.NewGuid().ToString()));

        services.AddSingleton(sp => new QdrantClient(vectorStoreEndpoint, port, useHttps));
        services.AddQdrantVectorStore();
    }
}