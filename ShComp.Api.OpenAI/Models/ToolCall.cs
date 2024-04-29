using System.Text.Json.Serialization;

namespace ShComp.Api.OpenAI.Models;

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
