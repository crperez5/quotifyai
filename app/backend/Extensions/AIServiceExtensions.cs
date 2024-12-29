namespace MinimalApi.Extensions;

internal static class AIServiceExtensions
{
    internal static IServiceCollection AddAIServices(this IServiceCollection services)
    {
        var azureAIEndpoint = Environment.GetEnvironmentVariable("AZURE_AI_ENDPOINT") ?? throw new InvalidOperationException("Azure AI endpoint is not set.");
        var chatDeploymentName = Environment.GetEnvironmentVariable("ChatDeploymentName") ?? throw new InvalidOperationException("Chat deployment name is not set.");
        var embeddingsDeploymentName = Environment.GetEnvironmentVariable("EmbeddingsDeploymentName") ?? throw new InvalidOperationException("Embeddings deployment name is not set.");

        var kernelBuilder = services.AddKernel();
        kernelBuilder.Services.AddLogging(configure => configure.AddConsole());
        kernelBuilder.Services.AddLogging(configure => configure.SetMinimumLevel(LogLevel.Trace));

        kernelBuilder.AddAzureOpenAIChatCompletion(
            chatDeploymentName,
            azureAIEndpoint,
            new DefaultAzureCredential());

#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        kernelBuilder.AddAzureOpenAITextEmbeddingGeneration(
            embeddingsDeploymentName,
            azureAIEndpoint,
            new DefaultAzureCredential());
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

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