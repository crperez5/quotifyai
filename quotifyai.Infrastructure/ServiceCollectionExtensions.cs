using Amazon.DynamoDBv2;
using Microsoft.Extensions.DependencyInjection;
using quotifyai.Core.Quotes.SaveQuote;
using quotifyai.Core.Shared;
using quotifyai.Infrastructure.Databases;

namespace quotifyai.Infrastructure;

public static class ServiceCollectionExtensions
{
    private const string _DefaultTableName = "quotes";
    private const string _DefaultRegion = "eu-west-3";
    private const string _DefaultEndpoint = "";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeService, DateTimeService>();
        return services;
    }

    public static IServiceCollection AddQuotesService(this IServiceCollection services)
    {
        string quotesTableName = Environment.GetEnvironmentVariable("QUOTES_TABLE_NAME") ?? _DefaultTableName;
        string awsRegion = Environment.GetEnvironmentVariable("AWS_REGION") ?? _DefaultRegion;
        string dynamoDbEndpoint = Environment.GetEnvironmentVariable("DYNAMODB_ENDPOINT") ?? _DefaultEndpoint;

        var dynamoDbConfig = new AmazonDynamoDBConfig
        {
            RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsRegion)
        };

        if (!string.IsNullOrEmpty(dynamoDbEndpoint))
        {
            dynamoDbConfig.ServiceURL = dynamoDbEndpoint;
        }

        services.AddSingleton<IAmazonDynamoDB>(new AmazonDynamoDBClient(dynamoDbConfig));
        services.AddSingleton<IDynamoDBTableFactory, DynamoDBTableFactory>();

        services.AddSingleton<IQuotesService>(provider =>
        {
            var dynamoDbClient = provider.GetRequiredService<IAmazonDynamoDB>();
            var dynamoDbTableFactory = provider.GetRequiredService<IDynamoDBTableFactory>();
            var dateTimeService = provider.GetRequiredService<IDateTimeService>();
            return new DynamoDBQuotesService(
                dateTimeService,
                dynamoDbClient,
                dynamoDbTableFactory,
                quotesTableName);
        });

        return services;
    }
}