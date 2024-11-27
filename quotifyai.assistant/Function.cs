using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace quotifyai.assistant;

public class Function
{
    private const string _DefaultTableName = "quotes";
    private const string _DefaultRegion = "eu-west-3";
    private const string _DefaultEndpoint = "";

    private static readonly AmazonDynamoDBClient _dynamoDbClient;

    public async Task FunctionHandler(ILambdaContext context)
    {
        await SaveQuote();
    }

    private static async Task SaveQuote()
    {
        string tableName = Environment.GetEnvironmentVariable("QUOTES_TABLE_NAME") ?? _DefaultTableName;
        var table = Table.LoadTable(_dynamoDbClient, tableName);

        var quote = new Document
        {
            ["quoteId"] = Guid.NewGuid().ToString(),
            ["createdDate"] = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ssZ"),
            ["documentData"] = "example"
        };

        await table.PutItemAsync(quote);
    }

    static Function()
    {
        string dynamoDbEndpoint = Environment.GetEnvironmentVariable("DYNAMODB_ENDPOINT") ?? _DefaultEndpoint;
        string region = Environment.GetEnvironmentVariable("AWS_REGION") ?? _DefaultRegion; 

        if (!string.IsNullOrEmpty(dynamoDbEndpoint))
        {
            _dynamoDbClient = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region),
                ServiceURL = dynamoDbEndpoint
            });
        }
        else
        {
            _dynamoDbClient = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region)
            });
        }
    }
}
