using System.Threading.Channels;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

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

                    Include quotes to the relevant information when used to give an answer.

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

            var assistantMessage = string.Empty;
            await foreach (var word in response.ConfigureAwait(false))
            {
                assistantMessage += word;

                await hubContext.Clients.Group(request.ConversationId)
                    .SendAsync("ReceiveMessage", new
                    {
                        conversationId = request.ConversationId,
                        messageId = request.Message.Id,
                        content = word.ToString()
                    }, stoppingToken);
            }

            var newAssistantMessage = new Message
            {
                Role = Role.Assistant,
                Content = assistantMessage,
            };

            var conversation = await db.Conversations.FindAsync([request.ConversationId]);
            conversation!.AddMessage(newAssistantMessage);
            await db.SaveChangesAsync();

            await hubContext.Clients.Group(request.ConversationId)
                .SendAsync("ReceiveMessage", new
                {
                    conversationId = request.ConversationId,
                    content = string.Empty,                    
                    type = "End",
                }, stoppingToken);
        }
    }
}