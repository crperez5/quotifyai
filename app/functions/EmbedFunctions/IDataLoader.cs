namespace EmbedFunctions;

public interface IDataLoader
{
    /// <summary>
    /// Load the text from a PDF file into the data store.
    /// </summary>
    /// <param name="pdfPath">The pdf path to load.</param>
    /// <param name="pdfStream">The pdf file to load.</param>
    /// <param name="batchSize">Maximum number of parallel threads to generate embeddings and upload records.</param>
    /// <param name="betweenBatchDelayInMs">The number of milliseconds to delay between batches to avoid throttling.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests.</param>
    /// <returns>An async task that completes when the loading is complete.</returns>
    Task LoadPdf(string pdfPath, Stream pdfStream, int batchSize, int betweenBatchDelayInMs, CancellationToken cancellationToken);
}