using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;

namespace quotifyai.Infrastructure.Databases;

public class DynamoDBTableFactory : IDynamoDBTableFactory
{
    public IDynamoDBTable LoadTable(IAmazonDynamoDB client, string tableName)
    {
        var internalTable = Table.LoadTable(client, tableName);
        return new DynamoDBTable(internalTable);
    }
}
