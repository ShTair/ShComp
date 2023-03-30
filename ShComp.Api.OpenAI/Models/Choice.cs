using System.Text.Json.Serialization;

namespace ShComp.Api.OpenAI.Models;

public class Choice
{
    [JsonPropertyName("message")]
    public Message Message { get; set; } = default!;

    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; } = default!;

    [JsonPropertyName("index")]
    public int Index { get; set; }
}
