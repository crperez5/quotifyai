using Microsoft.Extensions.Configuration;

namespace EmbedFunctions;

public class EmbeddingFunction(ILogger<EmbeddingFunction> logger, IConfiguration configuration)
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<EmbeddingFunction> _logger = logger;

    [Function(nameof(EmbeddingFunction))]
    public async Task Run([BlobTrigger("instructions/{name}", Connection = "AzureWebJobsStorage")] Stream stream, string name)
    {
        var valueFromKeyVault = await Task.FromResult(_configuration["DummySecret"]);
        _logger.LogInformation($"C# Blob trigger function Read Secret From KeyVault \n Value: {valueFromKeyVault} \n");

        using var blobStreamReader = new StreamReader(stream);
        var content = await blobStreamReader.ReadToEndAsync();
        _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n");
    }
}