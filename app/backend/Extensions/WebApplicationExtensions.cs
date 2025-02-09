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
        api.MapGet("/conversations", OnGetConversationsAsync);
        api.MapPost("/conversations", OnStartConversationAsync);
        api.MapDelete("/conversations/{id}", OnDeleteConversationAsync);
        api.MapPost("/conversations/{id}/messages", OnAddMessageAsync);
        return app;
    }

    public static async Task<IResult> OnAddMessageAsync(
        string id,
        Message message,
        [FromServices] AppDbContext appDbContext,
        [FromServices] MessageProcessingService processingService,
        CancellationToken cancellationToken)
    {
        var conversation = await appDbContext.Conversations.FindAsync([id]);
        if (conversation is null)
        {
            return TypedResults.BadRequest("The conversation does not exist");
        }
        conversation.AddMessage(message);

        await appDbContext.SaveChangesAsync();

        _ = processingService.QueueMessageForProcessing(conversation.Id, message);

        return Results.Created($"/conversations/{conversation.Id}/messages/{message.Id}", message);
    }

    public static async Task<IResult> OnGetConversationsAsync([FromServices] AppDbContext dbContext)
    {
        var conversations = await dbContext.Conversations.ToListAsync();
        return TypedResults.Ok(conversations);
    }

    public static async Task<IResult> OnDeleteConversationAsync([FromServices] AppDbContext appDbContext, string id)
    {
        var conversation = await appDbContext.Conversations.FindAsync([id]);
        if (conversation is null)
        {
            return TypedResults.NotFound();
        }

        appDbContext.Conversations.Remove(conversation);
        await appDbContext.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    public static async Task<IResult> OnStartConversationAsync(
        [FromServices] AppDbContext dbContext, 
        [FromServices] MessageProcessingService processingService,
        Conversation conversation)
    {
        if (conversation.Messages.Count == 0)
        {
            return TypedResults.BadRequest("Cannot start a conversation without a message");
        }

        await dbContext.Conversations.AddAsync(conversation);
        await dbContext.SaveChangesAsync();
        var message = conversation.Messages.First();
        _ = processingService.QueueMessageForProcessing(conversation.Id, message);
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
