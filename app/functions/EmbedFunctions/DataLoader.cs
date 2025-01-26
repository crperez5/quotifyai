using System.Net;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;

namespace EmbedFunctions;

internal sealed class DataLoader(
    UniqueKeyGenerator<Guid> uniqueKeyGenerator,
    IVectorStoreRecordCollection<Guid, TextSnippet<Guid>> vectorStoreRecordCollection,
    ITextEmbeddingGenerationService textEmbeddingGenerationService,
    ILogger<DataLoader> logger) : IDataLoader
{
    public async Task LoadPdf(string pdfPath, Stream pdfStream, int batchSize, int betweenBatchDelayInMs, CancellationToken cancellationToken)
    {
        await vectorStoreRecordCollection.CreateCollectionIfNotExistsAsync(cancellationToken);

        var sections = LoadTextAndImages(pdfStream, cancellationToken);
        var batches = sections.Chunk(batchSize);

        foreach (var batch in batches)
        {
            // Map each paragraph to a TextSnippet and generate an embedding for it.
            var recordTasks = batch.Select(async content => new TextSnippet<Guid>
            {
                Key = uniqueKeyGenerator.GenerateKey(),
                Text = content.Text,
                ReferenceDescription = $"{new FileInfo(pdfPath).Name}#page={content.PageNumber}",
                ReferenceLink = $"{new Uri(new FileInfo(pdfPath).FullName).AbsoluteUri}#page={content.PageNumber}",
                TextEmbedding = await GenerateEmbeddingsWithRetryAsync(textEmbeddingGenerationService, content.Text!, cancellationToken: cancellationToken).ConfigureAwait(false)
            });

            var records = await Task.WhenAll(recordTasks).ConfigureAwait(false);
            var upsertedKeys = vectorStoreRecordCollection.UpsertBatchAsync(records, cancellationToken: cancellationToken);
            await foreach (var key in upsertedKeys.ConfigureAwait(false))
            {
                logger.LogInformation("Upserted record '{Key}' into VectorDB", key);
            }

            await Task.Delay(betweenBatchDelayInMs, cancellationToken).ConfigureAwait(false);
        }

    }

    /// <summary>
    /// A simple retry mechanism to embedding generation.
    /// </summary>
    /// <param name="textEmbeddingGenerationService">The embedding generation service.</param>
    /// <param name="text">The text to generate the embedding for.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests.</param>
    /// <returns>The generated embedding.</returns>
    private async Task<ReadOnlyMemory<float>> GenerateEmbeddingsWithRetryAsync(ITextEmbeddingGenerationService textEmbeddingGenerationService, string text, CancellationToken cancellationToken)
    {
        var tries = 0;

        while (true)
        {
            try
            {
                return await textEmbeddingGenerationService.GenerateEmbeddingAsync(text, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (HttpOperationException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
            {
                tries++;

                if (tries < 3)
                {
                    logger.LogError("Failed to generate embedding. Error: {Error}", ex);
                    logger.LogInformation("Retrying embedding generation...");
                    await Task.Delay(10_000, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    throw;
                }
            }
        }
    }

    /// <summary>
    /// Read the text from each page in the provided PDF file.
    /// </summary>
    /// <param name="pdfStream">The pdf stream to read the text from.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests.</param>
    /// <returns>The text from the pdf file, plus the page number that each is on.</returns>
    private static IEnumerable<RawContent> LoadTextAndImages(Stream pdfStream, CancellationToken cancellationToken)
    {
        using PdfDocument document = PdfDocument.Open(pdfStream);
        foreach (Page page in document.GetPages())
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            var blocks = DefaultPageSegmenter.Instance.GetBlocks(page.GetWords());
            foreach (var block in blocks)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                yield return new RawContent { Text = block.Text, PageNumber = page.Number };
            }
        }
    }
}