using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using quotifyai.Core.Quotes.SaveQuote;
using quotifyai.Core.Shared;

namespace quotifyai.Infrastructure.Databases;

public class DynamoDBQuotesService(
    IDateTimeService dateTimeService,
    IAmazonDynamoDB dynamoDbClient,
    IDynamoDBTableFactory tableFactory,
    string tableName) : IQuotesService
{
    private readonly IDynamoDBTable _table = tableFactory.LoadTable(dynamoDbClient, tableName);
    private readonly IDateTimeService _dateTimeService = dateTimeService;

    public async Task SaveQuoteAsync(string quoteData)
    {
        var quote = new Document
        {
            ["quoteId"] = Guid.NewGuid().ToString(),
            ["createdDate"] = _dateTimeService.GetCurrentDateTimeUtc().ToString(Constants.QuoteDateTimeFormat),
            ["documentData"] = quoteData
        };

        await _table.PutItemAsync(quote);
    }
}
