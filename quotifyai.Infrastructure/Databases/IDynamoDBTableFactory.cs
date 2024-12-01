using Amazon.DynamoDBv2;

namespace quotifyai.Infrastructure.Databases;

internal interface IDynamoDBTableFactory
{
    IDynamoDBTable LoadTable(IAmazonDynamoDB client, string tableName);
}
