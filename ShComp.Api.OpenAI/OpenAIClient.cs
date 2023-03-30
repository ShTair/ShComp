using RestSharp;
using RestSharp.Authenticators;
using ShComp.Api.OpenAI.Models;

namespace ShComp.Api.OpenAI;

public sealed class OpenAIClient : IDisposable
{
    private readonly RestClient _client;

    public OpenAIClient(string apiKey, int timeout = 30000)
    {
        _client = InitializeClient(apiKey, timeout, opts => new RestClient(opts));
    }

    public OpenAIClient(HttpClient client, string apiKey, int timeout = 30000)
    {
        _client = InitializeClient(apiKey, timeout, opts => new RestClient(client, opts));
    }

    private static RestClient InitializeClient(string apiKey, int timeout, Func<RestClientOptions, RestClient> creator)
    {
        var options = new RestClientOptions(new Uri("https://api.openai.com/v1/chat/"))
        {
            Authenticator = new JwtAuthenticator(apiKey),
            MaxTimeout = timeout,
        };

        return creator(options);
    }

    public void Dispose()
    {
        _client?.Dispose();
    }

    public async Task<Response?> CompletionsAsync(Request request)
    {
        return await _client.PostJsonAsync<Request, Response>("completions", request);
    }
}
