using Amazon.DynamoDBv2.DocumentModel;

namespace quotifyai.Infrastructure.Databases;

public class DynamoDBTable(Table table) : IDynamoDBTable
{
    private readonly Table _table = table;

    public Task<Document> PutItemAsync(Document doc, CancellationToken cancellationToken = default)
    {
        return _table.PutItemAsync(doc, cancellationToken);
    }
}
