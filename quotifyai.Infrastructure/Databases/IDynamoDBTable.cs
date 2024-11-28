using Amazon.DynamoDBv2.DocumentModel;

namespace quotifyai.Infrastructure.Databases;

public interface IDynamoDBTable
{
    Task<Document> PutItemAsync(Document doc, CancellationToken cancellationToken = default);
}