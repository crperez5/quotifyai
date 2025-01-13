using Newtonsoft.Json;

namespace MinimalApi.Conversations.Models;

internal sealed class Message
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public required Role Role { get; init; }
    public required string Content { get; init; }
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}