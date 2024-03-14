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

    public static StringMessage CreateTool(string content) => new(MessageRoleTypes.Tool, content);
}

public class StringMessage : Message
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("tool_call_id")]
    public string? ToolCallId { get; set; }

    [JsonPropertyName("tool_calls")]
    public List<ToolCall>? ToolCalls { get; set; }

    public StringMessage(string role, string content, string? toolCallId = default)
    {
        Role = role;
        Content = content;
        ToolCallId = toolCallId;
    }

    public override string ToString() => $"{Role}: {Content}";
}

public class ToolCall
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("function")]
    public required CallFunction Function { get; set; }
}

public class CallFunction
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("arguments")]
    public required string Arguments { get; set; }
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
