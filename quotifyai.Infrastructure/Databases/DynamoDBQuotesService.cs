using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using quotifyai.Core.Quotes;
using quotifyai.Core.Shared;

namespace quotifyai.Infrastructure.Databases;

internal sealed class DynamoDBQuotesService(
    IDateTimeService dateTimeService,
    IAmazonDynamoDB dynamoDbClient,
    IDynamoDBTableFactory tableFactory,
    string tableName) : IQuotesService
{
    public async Task SaveQuoteAsync(string quoteData)
    {
        var table = tableFactory.LoadTable(dynamoDbClient, tableName);

        var quote = new Document
        {
            ["quoteId"] = Guid.NewGuid().ToString(),
            ["createdDate"] = dateTimeService.GetCurrentDateTimeUtc().ToString(Constants.QuoteDateTimeFormat),
            ["documentData"] = quoteData
        };

        await table.PutItemAsync(quote);
    }
}
