namespace MinimalApi.Services;

internal interface IAzureBlobStorageService
{
    Task<UploadDocumentsResponse> UploadFilesAsync(IEnumerable<IFormFile> files, CancellationToken cancellationToken);
}