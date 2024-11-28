using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using quotifyai.Core.Quotes.SaveQuote;
using quotifyai.DependencyInjection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace quotifyai.Assistant;

public class Function
{
    private static readonly IServiceProvider _serviceProvider;
    private readonly IQuotesService _quotesService;

    public Function(): this(null)
    {
    }
    
    internal Function(IQuotesService? quotesService = null)
    {
        _quotesService = quotesService ?? _serviceProvider.GetRequiredService<IQuotesService>();
    }

    static Function()
    {
        _serviceProvider = new ServiceCollection()
            .AddQuotifyAiServices()
            .BuildServiceProvider();
    }

    public async Task FunctionHandler(ILambdaContext context)
    {
        await _quotesService.SaveQuoteAsync("data");
    }
}
