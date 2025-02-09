using System.Text.Json;
using System.Text.Json.Serialization;

namespace MinimalApi.Conversations.Models;

public sealed class RoleConverter : JsonConverter<Role>
{
    public override Role Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value?.ToLower() switch
        {
            "user" => Role.User,
            "assistant" => Role.Assistant,
            _ => throw new JsonException($"Invalid role value: {value}")
        };
    }

    public override void Write(Utf8JsonWriter writer, Role value, JsonSerializerOptions options)
    {
        var roleString = value switch
        {
            Role.User => "user",
            Role.Assistant => "assistant",
            _ => throw new JsonException($"Invalid role value: {value}")
        };
        writer.WriteStringValue(roleString);
    }
}