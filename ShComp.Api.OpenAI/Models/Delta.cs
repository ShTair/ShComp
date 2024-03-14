using System.Text.Json.Serialization;

namespace ShComp.Api.OpenAI.Models;

public class Delta
{
    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("tool_calls")]
    public ToolCallDelta[]? ToolCalls { get; set; }
}

public class ToolCallDelta
{
    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("function")]
    public required CallFunctionDelta Function { get; set; }
}

public class CallFunctionDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("arguments")]
    public required string Arguments { get; set; }
}
