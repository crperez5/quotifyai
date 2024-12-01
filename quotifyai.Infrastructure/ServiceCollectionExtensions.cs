using Amazon.DynamoDBv2;
using Microsoft.Extensions.DependencyInjection;
using quotifyai.Core.Emails;
using quotifyai.Core.Quotes;
using quotifyai.Core.Shared;
using quotifyai.Infrastructure.Databases;
using quotifyai.Infrastructure.Email;

namespace quotifyai.Infrastructure;

public static class ServiceCollectionExtensions
{
    private const string _QuotesTableName = "quotes";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeService, DateTimeService>();
        return services;
    }

    public static IServiceCollection AddEmailService(this IServiceCollection services)
    {
        var emailServiceOptions = EmailServiceOptions.Create();
        services.AddTransient<IImapClientFactory, ImapClientFactory>();
        services.AddTransient<IEmailService>(s =>
        {
            IImapClientFactory imapClientFactory = s.GetRequiredService<IImapClientFactory>();
            var emailService =  new EmailService(emailServiceOptions, imapClientFactory);
            return emailService;        
        });

        return services;
    }

    public static IServiceCollection AddQuotesService(this IServiceCollection services)
    {
        string awsRegion = Environment.GetEnvironmentVariable("AWS_REGION")
            ?? throw new InvalidOperationException("AWS_REGION environment variable is required.");

        string dynamoDbEndpoint = Environment.GetEnvironmentVariable("DYNAMODB_ENDPOINT")
            ?? throw new InvalidOperationException("DYNAMODB_ENDPOINT environment variable is required.");

        var dynamoDbConfig = new AmazonDynamoDBConfig
        {
            RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsRegion)
        };

        if (!string.IsNullOrEmpty(dynamoDbEndpoint))
        {
            dynamoDbConfig.ServiceURL = dynamoDbEndpoint;
        }

        services.AddSingleton<IDateTimeService, DateTimeService>();
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
                _QuotesTableName);
        });

        return services;
    }
}