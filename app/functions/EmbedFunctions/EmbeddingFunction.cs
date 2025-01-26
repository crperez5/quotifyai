namespace EmbedFunctions;

public class EmbeddingFunction(ILogger<EmbeddingFunction> logger, IDataLoader dataLoader)
{
    private readonly ILogger<EmbeddingFunction> _logger = logger;
    private readonly IDataLoader _dataLoader = dataLoader;

    [Function(nameof(EmbeddingFunction))]
    public async Task Run([BlobTrigger("instructions/{name}", Connection = "AzureWebJobsStorage")] Stream stream, string name, CancellationToken ct)
    {
        await _dataLoader.LoadPdf($"instructions/{name}", stream, batchSize: 10, betweenBatchDelayInMs: 1000, ct);
        
        _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n");
    }
}