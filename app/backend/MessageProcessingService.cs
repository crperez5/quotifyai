using System.Threading.Channels;
using HandlebarsDotNet;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using MinimalApi.Plugins.Prices;

namespace MinimalApi;

internal sealed class MessageProcessingService(IHubContext<ChatHub> hubContext, IServiceProvider serviceProvider) : BackgroundService
{
    private readonly Channel<MessageProcessingRequest> _channel = Channel.CreateUnbounded<MessageProcessingRequest>();

    public async Task QueueMessageForProcessing(string conversationId, Message message)
    {
        await _channel.Writer.WriteAsync(new MessageProcessingRequest(conversationId, message));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var request in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var kernel = scope.ServiceProvider.GetRequiredService<Kernel>();
            var vectorStoreTextSearch = scope.ServiceProvider.GetRequiredService<VectorStoreTextSearch<TextSnippet<Guid>>>();
            kernel.Plugins.Add(vectorStoreTextSearch.CreateWithGetTextSearchResults("SearchPlugin"));
            kernel.Plugins.AddFromType<PricesPlugin>("Prices");

            var conversation = await db.Conversations.FindAsync([request.ConversationId]);

            var response = kernel.InvokePromptStreamingAsync(
                promptTemplate: """
                    Please, use the below information to answer the question:
                    {{#with (SearchPlugin-GetTextSearchResults question)}}
                      {{#each this}}
                        Name: {{Name}}
                        Value: {{Value}}
                        Link: {{Link}}
                        -----------------
                      {{/each}}
                    {{/with}}

                    Question: {{question}}
                    """,
                arguments: new KernelArguments(new PromptExecutionSettings
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
                })
                {
                    { "question", request.Message.Content },
                },
                templateFormat: "handlebars",
                promptTemplateFactory: new HandlebarsPromptTemplateFactory(),
                cancellationToken: stoppingToken);

            var newAssistantMessage = new Message
            {
                Role = Role.Assistant,
            };

            var assistantMessageContent = string.Empty;
            var firstMessageToken = true;
            await foreach (var token in response.ConfigureAwait(false))
            {
                assistantMessageContent += token;

                await hubContext.Clients.Group(request.ConversationId)
                    .SendAsync("ReceiveToken", new
                    {
                        conversationId = request.ConversationId,
                        messageId = newAssistantMessage.Id,
                        content = token.ToString(),
                        role = "assistant",
                        createdAt = newAssistantMessage.CreatedAt,
                        type = firstMessageToken ? "startAssistantMessage" : "continueAssistantMessage"
                    }, stoppingToken);

                firstMessageToken = false;
            }

            newAssistantMessage.Content = assistantMessageContent;

            conversation!.AddMessage(newAssistantMessage);
            await db.SaveChangesAsync();

            if (conversation.Messages.Count > 2)
            {
                continue;
            }

            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            var chatHistory = new ChatHistory();
            var promptTemplate = """
                The user wants to calculate a quote. 
                Summarize what the quote is about in less than 18 characters.
                If it doesn't fit, truncate the words or try abbreviating them.
                Do not add any additional comments.
                Always use the user's language.
                User Input: {{$userInput}}
                """;
            var prompt = promptTemplate.Replace("{{$userInput}}", request.Message.Content);
            chatHistory.AddUserMessage(prompt);
            var streamingResponse = chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory);
            var firstTitleToken = true;
            var conversationTitle = string.Empty;
            await foreach (var token in streamingResponse)
            {
                conversationTitle += token;
                await hubContext.Clients.Group(request.ConversationId)
                    .SendAsync("ReceiveToken", new
                    {
                        conversationId = request.ConversationId,
                        title = token.ToString(),
                        role = "assistant",
                        type = firstTitleToken ? "startConversationTitle" : "continueConversationTitle"
                    }, stoppingToken);
                firstTitleToken = false;
            }

            conversation.Title = conversationTitle;
            await db.SaveChangesAsync();
        }
    }
}