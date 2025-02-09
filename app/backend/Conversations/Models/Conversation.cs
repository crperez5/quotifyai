namespace MinimalApi.Conversations.Models;

internal sealed class Conversation
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime LastMessageAt { get; private set; } = DateTime.UtcNow;
    public List<Message> Messages { get; init; } = [];

    public void AddMessage(Message message)
    {
        LastMessageAt = DateTime.UtcNow;
        Messages.Add(message);
    }
}