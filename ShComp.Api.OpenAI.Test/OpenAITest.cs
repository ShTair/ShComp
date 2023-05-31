using Microsoft.Extensions.Configuration;
using ShComp.Api.OpenAI.Models;
using System.Diagnostics;

namespace ShComp.Api.OpenAI.Test;

public class OpenAITest
{
    private readonly OpenAIClient _client;

    public OpenAITest()
    {
        var configuration = new ConfigurationBuilder().AddUserSecrets<OpenAITest>().Build();
        _client = new OpenAIClient(configuration["OpenAI:ApiKey"]!);
    }

    [Fact]
    public async Task CompletionsTest()
    {
        var messages = new[] { Message.CreateUser("こんにちは") };
        var request = Request.Define().WithGpt3_5Turbo().WithMaxTokens(1000).WithTemperature(0).Create(messages);

        var response = await _client.CompletionsAsync(request);

        Assert.NotNull(response);
        Assert.NotEmpty(response.Choices);
    }

    [Fact]
    public async Task CompletionsStreamTest()
    {
        var messages = new[] { Message.CreateUser("こんにちは") };
        var request = Request.Define().WithGpt3_5Turbo().WithMaxTokens(1000).WithTemperature(0).Create(messages);

        await foreach (var chunk in _client.CompletionsStreamAsync(request))
        {
            if (chunk is { Choices: [{ Delta: { Content: { } content } }, ..] })
            {
                Debug.Write(content);
            }
        }
    }
}
