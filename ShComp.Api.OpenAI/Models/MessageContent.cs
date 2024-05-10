using System.Text.Json.Serialization;

namespace ShComp.Api.OpenAI.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(TextContent), "text")]
[JsonDerivedType(typeof(ImageUrlContent), "image_url")]
public interface IMessageContent { }

public class TextContent : IMessageContent
{
    [JsonPropertyName("text")]
    public string Text { get; set; }

    public TextContent(string text)
    {
        Text = text;
    }

    public static implicit operator TextContent(string text) => new(text);
}

public class ImageUrlContent : IMessageContent
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

    public static async Task<ImageUrlContent> FromFileAsync(string path, string? detail = ImageUrlDetails.Auto)
    {
        var imageUrl = await ImageUrl.FromFileAsync(path);
        return new ImageUrlContent(imageUrl, detail);
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

    public static async Task<string> FromStreamAsync(Stream stream, string contentType, int count)
    {
        var buffer = new byte[count];
        await stream.ReadAsync(buffer);
        return $"data:{contentType};base64,{Convert.ToBase64String(buffer)}";
    }

    public static string FromBytes(string contentType, byte[] buffer)
    {
        return $"data:{contentType};base64,{Convert.ToBase64String(buffer)}";
    }
}

public static class ImageUrlDetails
{
    public const string? Auto = null;

    public const string High = "high";

    public const string Low = "low";
}
