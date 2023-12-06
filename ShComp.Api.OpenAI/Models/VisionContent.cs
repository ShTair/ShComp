using System.Text.Json.Serialization;

namespace ShComp.Api.OpenAI.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(TextContent), "text")]
[JsonDerivedType(typeof(ImageUrlContent), "image_url")]
public interface IVisionContent { }

public class TextContent : IVisionContent
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = default!;

    public TextContent(string text)
    {
        Text = text;
    }
}

public class ImageUrlContent : IVisionContent
{
    [JsonPropertyName("image_url")]
    public ImageUrl ImageUrl { get; set; } = default!;

    public ImageUrlContent(string url)
    {
        ImageUrl = new ImageUrl { Url = url };
    }

    public ImageUrlContent(string url, string? detail)
    {
        ImageUrl = new ImageUrl { Url = url, Detail = detail };
    }
}

public class ImageUrl
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;

    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    public static async Task<string> FromFileAsync(string path)
    {
        var contentType = Path.GetExtension(path) switch
        {
            ".jpg" or ".jpeg" or ".JPG" or ".JPEG" => "image/jpeg",
            ".png" or ".PNG" => "image/png",
            ".webp" or ".WEBP" => "image/webp",
            ".gif" or ".GIF" => "image/gif",
            _ => throw new Exception("不明な拡張子"),
        };

        var buffer = await File.ReadAllBytesAsync(path);
        return $"data:{contentType};base64,{Convert.ToBase64String(buffer)}";
    }
}

public static class ImageUrlDetails
{
    public const string? Auto = null;

    public const string High = "high";

    public const string Low = "low";
}
