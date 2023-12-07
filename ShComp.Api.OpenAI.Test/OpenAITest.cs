﻿using Microsoft.Extensions.Configuration;
using ShComp.Api.OpenAI.Models;
using System.Diagnostics;

namespace ShComp.Api.OpenAI.Test;

public class OpenAITest
{
    private readonly OpenAIClient _client;
    private readonly IConfiguration _configuration;

    public OpenAITest()
    {
        _configuration = new ConfigurationBuilder().AddUserSecrets<OpenAITest>().Build();
        _client = new OpenAIClient(_configuration["OpenAI:ApiKey"]!);
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
        var messages = new[] { Message.CreateUser("C#のコンテキストキーワードについて教えてください") };
        var request = Request.Define().WithGpt3_5Turbo().WithMaxTokens(1000).WithTemperature(0).Create(messages);

        try
        {
            await foreach (var chunk in _client.CompletionsStreamAsync(request))
            {
                if (chunk is { Choices: [{ Delta.Content: { } content }, ..] })
                {
                    Debug.Write($"{content},");
                }
            }
        }
        finally { Debug.WriteLine(""); }
    }

    [Fact]
    public async Task VisionCompletionsTest()
    {
        var messages = new[] { new VisionMessage(MessageRoleTypes.User, "これ何？", new ImageUrlContent(await ImageUrl.FromFileAsync(_configuration["OpenAI:ImageFilePath1"]!), ImageUrlDetails.High)) };

        //var messages = new[] { new VisionMessage(MessageRoleTypes.User, "今日の献立は何にしようかな？", new ImageUrlContent(await ImageUrl.FromFileAsync(_configuration["OpenAI:ImageFilePath2"]!), ImageUrlDetails.High)) };

        //var messages = new[] { new VisionMessage(MessageRoleTypes.User, "この図を、mermaid記法で書き直してください。", new ImageUrlContent(_configuration["OpenAI:ImageUrl"]!, ImageUrlDetails.High)) };

        //var messages = new[] { new VisionMessage(MessageRoleTypes.User, "この図を、mermaid記法で書き直してください。SequenceDiagramを使うのが適切だと思います。上部の青い四角は、担当チームを表しています。", new ImageUrlContent(_configuration["OpenAI:ImageUrl"]!, ImageUrlDetails.High)) };

        var request = Request.Define()
            .WithGpt4TurboWithVision()
            .WithMaxTokens(3000)
            .WithTemperature(0)
            .Create(messages);

        var response = await _client.CompletionsAsync(request);

        Assert.NotNull(response);
        Assert.NotEmpty(response.Choices);

        var content = response.Choices[0].Message!.Content;
        var cost = response.Usage!.PromptTokens * 0.00001 + response.Usage.CompletionTokens * 0.00003;
        var yen = cost * 150;
    }
}
