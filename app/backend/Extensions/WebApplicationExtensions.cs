namespace MinimalApi.Extensions;

internal static class WebApplicationExtensions
{
    internal static WebApplication MapApi(this WebApplication app)
    {
        var api = app.MapGroup("api");

        api.MapGet("/", OnGetAsync);

        return app;
    }

    private static Task<IResult> OnGetAsync(IConfiguration config)
    {
        return Task.FromResult<IResult>(TypedResults.Ok("Hello World!"));
    }
}