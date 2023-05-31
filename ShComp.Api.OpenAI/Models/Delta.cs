using System.Text.Json.Serialization;

namespace ShComp.Api.OpenAI.Models;

public class Delta
{
    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
