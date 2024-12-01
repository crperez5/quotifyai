using Amazon.DynamoDBv2.DocumentModel;

namespace quotifyai.Infrastructure.Databases;

internal interface IDynamoDBTable
{
    Task<Document> PutItemAsync(Document doc, CancellationToken cancellationToken = default);
}