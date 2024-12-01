using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using quotifyai.Core.Common;
using quotifyai.Core.Emails;
using quotifyai.Core.Quotes;
using quotifyai.DependencyInjection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace quotifyai.Assistant;

public class Function
{
    private static readonly IServiceProvider _serviceProvider;
    private readonly IDateTimeService _dateTimeService;
    private readonly IQuotesService _quotesService;
    private readonly IEmailService _emailService;

    public Function() : this(null, null)
    {
    }

    internal Function(
        IDateTimeService? dateTimeService = null,
        IQuotesService? quotesService = null,
        IEmailService? emailService = null)
    {
        _dateTimeService = dateTimeService ?? _serviceProvider.GetRequiredService<IDateTimeService>();
        _quotesService = quotesService ?? _serviceProvider.GetRequiredService<IQuotesService>();
        _emailService = emailService ?? _serviceProvider.GetRequiredService<IEmailService>();
    }

    static Function()
    {
        _serviceProvider = new ServiceCollection()
            .AddQuotifyAiServices()
            .BuildServiceProvider();
    }

    public async Task FunctionHandler(ILambdaContext context)
    {
        var utcNow = _dateTimeService.GetCurrentDateTimeUtc();
        var fromUtcDateTime = ExecutionTimeCalculator.GetLastExecutionUtcDateTimeRelativeTo(utcNow);
        Result<List<EmailContent>, string> emailsResult = await _emailService.GetEmailsAsync(fromUtcDateTime);
        if (emailsResult.IsError)
        {
            context.Logger.LogError($"Failed to fetch emails: {emailsResult.ToError()}");
            return;
        }
        await _quotesService.SaveQuoteAsync("data");
    }
}
