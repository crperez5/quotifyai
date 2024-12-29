using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel.ChatCompletion;

namespace MinimalApi.Extensions;

internal static class WebApplicationExtensions
{
    internal static WebApplication MapApi(this WebApplication app)
    {
        var api = app.MapGroup("api");

        api.MapGet("/", OnGetAsync);

        api.MapGet("/hello-ai", OnHelloAI);

        return app;
    }

    private static Task<IResult> OnGetAsync(IConfiguration config)
    {
        return Task.FromResult<IResult>(TypedResults.Ok("Hello World!"));
    }

    private static async Task<IResult> OnHelloAI([FromServices] Kernel kernel)
    {
        try
        {
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            ChatHistory history = [];
            history.AddUserMessage("Hola, qué tal?");
            var response = await chatCompletionService.GetChatMessageContentAsync(
                history,
                kernel: kernel
            );

            var content = response.Content;
            return TypedResults.Ok(content);
        }
        catch (Exception ex)
        {
            return TypedResults.Ok(new { Error = ex.Message, Details = ex.InnerException?.Message });
        }
    }
}