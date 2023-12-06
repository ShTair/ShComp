using System.Text.Json.Serialization;

namespace ShComp.Api.OpenAI.Models;

[JsonDerivedType(typeof(StringMessage))]
[JsonDerivedType(typeof(VisionMessage))]
public abstract class Message
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = default!;

    public static StringMessage CreateSystem(string content) => new(MessageRoleTypes.System, content);

    public static StringMessage CreateUser(string content) => new(MessageRoleTypes.User, content);

    public static StringMessage CreateAssistant(string content) => new(MessageRoleTypes.Assistant, content);
}

public class StringMessage : Message
{
    [JsonPropertyName("content")]
    public string Content { get; set; }

    public StringMessage(string role, string content)
    {
        Role = role;
        Content = content;
    }

    public override string ToString() => $"{Role}: {Content}";
}

public class VisionMessage : Message
{
    [JsonPropertyName("content")]
    public List<IVisionContent> Contents { get; set; }

    public VisionMessage(string role, List<IVisionContent> contents)
    {
        Role = role;
        Contents = contents;
    }

    public VisionMessage(string role, string text, params ImageUrlContent[] images)
    {
        Role = role;
        Contents = [new TextContent(text), .. images];
    }

    public VisionMessage(string role, string text, params string[] urls)
    {
        Role = role;
        Contents = [new TextContent(text), .. urls.Select(t => new ImageUrlContent(t))];
    }

    public override string ToString() => $"{Role}: {Contents.Count}";
}
