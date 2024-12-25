namespace MinimalApi.Extensions;

internal static class WebApplicationExtensions
{
    internal static WebApplication MapApi(this WebApplication app)
    {
        var api = app.MapGroup("api");

        api.MapGet("/", OnGetAsync);

        return app;
    }

    private static async Task<IResult> OnGetAsync(IConfiguration config)
    {
        var valueFromKeyVault = await Task.FromResult(config["DummySecret"]);
        var valueFromEnv = await Task.FromResult(config["AzureStorageAccountEndpoint"]);

        if (string.IsNullOrWhiteSpace(valueFromKeyVault))
        {
            return TypedResults.NotFound("Secret not found or not configured.");
        }
        
        return TypedResults.Ok(new {
            ValueFromKeyVault = valueFromKeyVault,
            ValueFromEnv = valueFromEnv
        });
    }
}