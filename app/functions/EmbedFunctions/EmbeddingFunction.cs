using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace EmbedFunctions;

public class EmbeddingFunction(ILogger<EmbeddingFunction> logger)
{
    private readonly ILogger<EmbeddingFunction> _logger = logger;

    [Function(nameof(EmbeddingFunction))]
    public async Task Run([BlobTrigger("instructions/{name}", Connection = "AzureWebJobsStorage")] Stream stream, string name)
    {
        using var blobStreamReader = new StreamReader(stream);
        var content = await blobStreamReader.ReadToEndAsync();
        _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");
    }
}
