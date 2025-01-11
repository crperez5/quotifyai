namespace MinimalApi.Conversations.Models;

internal sealed class Conversation
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public required string UserId { get; init; }
    public DateTime StartedAt { get; } = DateTime.UtcNow;
    public DateTime LastMessageAt { get; } = DateTime.UtcNow;
    public List<Message> Messages { get; set; } = [];
}