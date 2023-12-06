using System.Text.Json.Serialization;

namespace ShComp.Api.OpenAI.Models;

public class Choice
{
    [JsonPropertyName("message")]
    public StringMessage? Message { get; set; }

    [JsonPropertyName("delta")]
    public Delta? Delta { get; set; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; }

    [JsonPropertyName("index")]
    public int Index { get; set; }
}
