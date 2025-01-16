namespace MinimalApi.Conversations.Models;

internal sealed class Conversation
{
    private List<Message> _messages = [];

    public string Id { get; } = "f7666902-a3e7-4ea9-9d42-dda651185fef";
    public required string UserId { get; init; }
    public DateTime StartedAt { get; } = DateTime.UtcNow;
    public DateTime LastMessageAt { get; private set; } = DateTime.UtcNow;
    public List<Message> Messages => _messages;

    public void AddMessage(Message message)
    {
        LastMessageAt = DateTime.UtcNow;
        _messages.Add(message);
    }
}