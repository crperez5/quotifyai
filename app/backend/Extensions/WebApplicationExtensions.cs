using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel.ChatCompletion;

namespace MinimalApi.Extensions;

internal static class WebApplicationExtensions
{
    internal static WebApplication MapApi(this WebApplication app)
    {
        var api = app.MapGroup("api");

        app.MapGet("/antiforgery/token", OnGetAntiForgeryToken);
        app.MapPost("/upload", OnFileUpload);

        // chat
        api.MapPost("/conversations", OnStartConversationAsync);
        api.MapPost("/conversations/{id}/messages", OnAddMessageAsync);

        return app;
    }

    public static async Task<IResult> OnAddMessageAsync(
        string id,
        MessageRequest messageRequest,
        [FromServices] AppDbContext appDbContext,
        [FromServices] Kernel kernel)
    {
        var conversation = await appDbContext.Conversations.FindAsync([id]);
        if (conversation == null)
        {
            return TypedResults.BadRequest("The conversation does not exist");
        }

        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        ChatHistory history = [];
        foreach (var message in conversation.Messages)
        {
            if (message.Role == Role.User)
            {
                history.AddUserMessage(message.Content);
            } else 
            {
                history.AddAssistantMessage(message.Content);
            }
        }

        var newUserMessage = new Message
        {
            Role = Role.User,
            Content = messageRequest.Content,
        };

        conversation.AddMessage(newUserMessage);
        history.AddUserMessage(newUserMessage.Content);

        var response = await chatCompletionService.GetChatMessageContentAsync(history, kernel: kernel);
        var newAssistantMessage = new Message
        {
            Role = Role.Assistant,
            Content = response.Content!,
        };        
        conversation.AddMessage(newAssistantMessage);
        await appDbContext.SaveChangesAsync();

        return Results.Created($"/conversations/{conversation.Id}/messages/{newUserMessage.Id}", new List<Message> { newUserMessage, newAssistantMessage });
    }

    public static async Task<IResult> OnStartConversationAsync([FromServices] AppDbContext dbContext)
    {
        var conversation = new Conversation
        {
            UserId = "1",
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
