using ShComp.Api.OpenAI.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ShComp.Api.OpenAI;

public sealed partial class OpenAIClient : IDisposable
{
    private readonly bool _disposeClient;
    private readonly HttpClient _client;

    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public OpenAIClient(string apiKey, int timeout = 30000) : this(new HttpClient(), apiKey, timeout)
    {
        _disposeClient = true;
    }

    public OpenAIClient(HttpClient client, string apiKey, int timeout = 30000)
    {
        _client = client;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _client.BaseAddress = new Uri("https://api.openai.com/v1/chat/");
        _client.Timeout = TimeSpan.FromMilliseconds(timeout);

        _jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public void Dispose()
    {
        if (_disposeClient) _client.Dispose();
    }

    public async Task<Response?> CompletionsAsync(Request request)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, "completions");
        req.Content = JsonContent.Create(request, options: _jsonSerializerOptions);

        using var res = await _client.SendAsync(req);
        return await res.Content.ReadFromJsonAsync<Response>();
    }

    public async IAsyncEnumerable<Response> CompletionsStreamAsync(Request request)
    {
        request.Stream = true;
        using var req = new HttpRequestMessage(HttpMethod.Post, "completions");
        req.Content = JsonContent.Create(request, options: _jsonSerializerOptions);

        await foreach (var data in PostUsingServerSentEventsAsync(req))
        {
            if (data == "[DONE]") break;
            if (JsonSerializer.Deserialize<Response>(data) is not { } response) continue;
            yield return response;
        }
    }

    public async Task<Response?> CompletionsStreamAsync(Request request, Action<string> handler)
    {
        var message = StringMessage.CreateAssistant(null!);
        Response? response = null;
        StringBuilder? sb = null;
        string? finishReason = null;
        await foreach (var chunk in CompletionsStreamAsync(request))
        {
            if (chunk is { Choices: [{ Delta: { } delta } choice, ..] })
            {
                if (delta.Content is { } content)
                {
                    handler(content);
                    sb ??= new StringBuilder();
                    sb.Append(content);
                }

                if (delta.ToolCalls is { })
                {
                    if (message.ToolCalls is not { })
                    {
                        message.ToolCalls = [];
                    }

                    foreach (var toolCallDelta in delta.ToolCalls)
                    {
                        var index = toolCallDelta.Index;
                        ToolCall toolCall;
                        if (message.ToolCalls.Count <= index)
                        {
                            toolCall = Activator.CreateInstance<ToolCall>();
                            toolCall.Function = Activator.CreateInstance<CallFunction>();
                            toolCall.Function.Arguments = "";

                            message.ToolCalls.Add(toolCall);
                        }
                        else
                        {
                            toolCall = message.ToolCalls[index];
                        }

                        if (toolCallDelta.Id is { }) toolCall.Id = toolCallDelta.Id;
                        if (toolCallDelta.Type is { }) toolCall.Type = toolCallDelta.Type;
                        if (toolCallDelta.Function is { } functionDelta)
                        {
                            var function = toolCall.Function;

                            if (functionDelta.Name is { }) function.Name = functionDelta.Name;
                            if (functionDelta.Arguments is { }) function.Arguments += functionDelta.Arguments;
                        }
                    }
                }

                if (choice.FinishReason is { }) finishReason = choice.FinishReason;
            }

            if (response is null)
            {
                response = chunk;
                response.Choices[0].Delta = null;
                response.Choices[0].Message = message;
            }
        }

        {
            message.Content = sb?.ToString();
            if (response is { Choices: [{ } choice, ..] }) choice.FinishReason = finishReason;
        }

        return response;
    }

    [GeneratedRegex(@"(?<=^data:\s*)\S.*")]
    private static partial Regex DataRegex();

    private async IAsyncEnumerable<string> PostUsingServerSentEventsAsync(HttpRequestMessage request)
    {
        using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        while (await reader.ReadLineAsync() is { } line)
        {
            if (DataRegex().Match(line) is { Success: true } match)
            {
                yield return match.Value;
            }
        }
    }
}
