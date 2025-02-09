using System.Text.Json.Serialization;

namespace MinimalApi.Conversations.Models;

internal sealed class Message
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();

    public string Content { get; set; } = string.Empty;

    [JsonConverter(typeof(RoleConverter))]
    public required Role Role { get; init; }
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}