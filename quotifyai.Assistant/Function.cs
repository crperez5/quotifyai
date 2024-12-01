using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using quotifyai.Core.Emails;
using quotifyai.Core.Quotes;
using quotifyai.DependencyInjection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace quotifyai.Assistant;

public class Function
{
    private static readonly IServiceProvider _serviceProvider;
    private readonly IQuotesService _quotesService;
    private readonly IEmailService _emailService;

    public Function(): this(null, null)
    {
    }
    
    internal Function(
        IQuotesService? quotesService = null,
        IEmailService? emailService = null)
    {
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
        var emails = await _emailService.GetEmailsAsync(DateTime.Today);
        await _quotesService.SaveQuoteAsync("data");
    }
}
