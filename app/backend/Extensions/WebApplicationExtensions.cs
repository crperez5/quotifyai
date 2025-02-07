using Microsoft.AspNetCore.Mvc;

namespace MinimalApi.Extensions;

internal static class WebApplicationExtensions
{
    internal static WebApplication MapApi(this WebApplication app)
    {
        var api = app.MapGroup("api");

        api.MapGet("/antiforgery/token", OnGetAntiForgeryToken);

        api.MapPost("/upload", OnFileUpload);

        // chat
        api.MapPost("/conversations", OnStartConversationAsync);
        api.MapPost("/conversations/{id}/messages", OnAddMessageAsync);

        return app;
    }

    public static async Task<IResult> OnAddMessageAsync(
        string id,
        MessageRequest messageRequest,
        [FromServices] AppDbContext appDbContext,
        [FromServices] MessageProcessingService processingService,
        CancellationToken cancellationToken)
    {
        var conversation = await appDbContext.Conversations.FindAsync([id]);
        if (conversation == null)
        {
            return TypedResults.BadRequest("The conversation does not exist");
        }

        var message = new Message
        {
            Role = Role.User,
            Content = messageRequest.Content,
        };
        conversation.AddMessage(message);

        await appDbContext.SaveChangesAsync();

        _ = processingService.QueueMessageForProcessing(conversation.Id, message);

        return Results.Created($"/conversations/{conversation.Id}/messages/{message.Id}", message);
    }

    public static async Task<IResult> OnStartConversationAsync([FromServices] AppDbContext dbContext)
    {
        var conversation = new Conversation
        {
            UserId = "1",
            Title = "New Conversation"
        };

        await dbContext.Conversations.AddAsync(conversation);
        await dbContext.SaveChangesAsync();

        return TypedResults.Created($"/conversations/{conversation.Id}", conversation);
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
}
