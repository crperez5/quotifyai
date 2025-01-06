using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel.ChatCompletion;

namespace MinimalApi.Extensions;

internal static class WebApplicationExtensions
{
    internal static WebApplication MapApi(this WebApplication app)
    {
        var api = app.MapGroup("api");

        app.MapGet("/antiforgery/token", OnGetAntiForgeryToken);
        api.MapGet("/hello-ai", OnHelloAI);
        app.MapPost("/upload", OnFileUpload);

        return app;
    }

    private static string OnGetAntiForgeryToken([FromServices] IAntiforgery antiforgery, HttpContext context)
    {
        var tokens = antiforgery.GetAndStoreTokens(context);
        return tokens.RequestToken!;
    }

    private static async Task<IResult> OnFileUpload(IFormFile file, [FromServices] IAzureBlobStorageService azureBlobStorageService, CancellationToken ct)
    {
        if (file.Length > 0)
        {
            var result = await azureBlobStorageService.UploadFilesAsync([file], ct);

            return result.IsSuccessful ? Results.Ok(result) : Results.BadRequest(result.Error);
        }

        return Results.BadRequest("Invalid file.");
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