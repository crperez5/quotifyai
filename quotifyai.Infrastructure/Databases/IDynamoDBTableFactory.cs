using Amazon.DynamoDBv2;

namespace quotifyai.Infrastructure.Databases;

public interface IDynamoDBTableFactory
{
    IDynamoDBTable LoadTable(IAmazonDynamoDB client, string tableName);
}
