using System.Text.Json.Serialization;

namespace ShComp.Api.OpenAI.Models;

public class Message
{
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    public Message(string role, string content)
    {
        Role = role;
        Content = content;
    }

    public override string ToString() => $"{Role}: {Content}";

    public static Message CreateSystem(string content) => new(MessageRoleTypes.System, content);

    public static Message CreateUser(string content) => new(MessageRoleTypes.User, content);

    public static Message CreateAssistant(string content) => new(MessageRoleTypes.Assistant, content);
}
