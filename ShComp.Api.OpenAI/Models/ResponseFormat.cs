using System.Text.Json.Serialization;

namespace ShComp.Api.OpenAI.Models;

public class ResponseFormat
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }
}

public static class ResponseFormatType
{
    public const string JsonObject = "json_object";
}
