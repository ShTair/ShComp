using Microsoft.Extensions.Configuration;
using ShComp.Api.OpenAI.Models;
using System.Diagnostics;
using System.Text.Json.Serialization;

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
        var messages = new List<Message>();
        messages.Add(new ArrayMessage(MessageRoleTypes.User, [
            await ImageUrlContent.FromFileAsync(_configuration["OpenAI:ImageFilePath1"]!),
            new TextContent("この画像、なんでしょうか。")]));

        var request = Request.Define().WithGpt4Turbo().WithMaxTokens(1000).WithTemperature(0).Create(messages);

        var response = await _client.CompletionsAsync(request);

        Assert.NotNull(response);
        Assert.NotEmpty(response.Choices);
    }

    [Fact]
    public async Task CompletionsStreamTest()
    {
        var messages = new List<Message>();
        messages.Add(new ArrayMessage(MessageRoleTypes.User, [
            await ImageUrlContent.FromFileAsync(_configuration["OpenAI:ImageFilePath1"]!),
            new TextContent("この画像、なんでしょうか。")]));

        var request = Request.Define().WithGpt4Turbo().WithMaxTokens(1000).WithTemperature(0).Create(messages);

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
        //var messages = new[] { new VisionMessage(MessageRoleTypes.User, "これ何？", new ImageUrlContent(await ImageUrl.FromFileAsync(_configuration["OpenAI:ImageFilePath1"]!), ImageUrlDetails.High)) };

        var messages = new[] { new ArrayMessage(MessageRoleTypes.User,[
            (TextContent)"今日の献立は何にしようかな？",
            new ImageUrlContent(await ImageUrl.FromFileAsync(_configuration["OpenAI:ImageFilePath2"]!), ImageUrlDetails.High)]) };

        //var messages = new[] { new VisionMessage(MessageRoleTypes.User, "この図を、mermaid記法で書き直してください。", new ImageUrlContent(_configuration["OpenAI:ImageUrl"]!, ImageUrlDetails.High)) };

        //var messages = new[] { new VisionMessage(MessageRoleTypes.User, "この図を、mermaid記法で書き直してください。SequenceDiagramを使うのが適切だと思います。上部の青い四角は、担当チームを表しています。", new ImageUrlContent(_configuration["OpenAI:ImageUrl"]!, ImageUrlDetails.High)) };

        var request = Request.Define()
            .WithGpt4Turbo()
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

    [Fact]
    public async Task FunctionTest()
    {
        var tools = Tools.Define()
            .AddFunction("display_message")
                .WithDescription("メッセージを画面に表示します。現在のメッセージは上書きされます。")
                .AddStringProperty("message", true)
                    .WithDescription("表示するメッセージ")
            .AddFunction("get_message")
                .WithDescription("表示しているメッセージを取得します。")
            .GetTools();

        var messages = new List<Message> { StringMessage.CreateUser("画面のメッセージの末尾に、こんにちはって追加して。") };

        var request = Request.Define()
            .WithGpt4Turbo()
            .WithMaxTokens(3000)
            .WithTemperature(0)
            .Create(messages, tools);

        var response = await _client.CompletionsAsync(request);

        messages.Add(response!.Choices[0].Message!);
        messages.Add(StringMessage.CreateTool("がんばれ", response!.Choices[0].Message!.ToolCalls![0].Id));

        request = Request.Define()
            .WithGpt4Turbo()
            .WithMaxTokens(3000)
            .WithTemperature(0)
            .Create(messages, tools);

        response = await _client.CompletionsAsync(request);
    }

    [Fact]
    public async Task FunctionStreamTest()
    {
        var tools = Tools.Define()
            .AddFunction("display_message")
                .WithDescription("メッセージを画面に表示します。現在のメッセージは上書きされます。")
                .AddStringProperty("message", true)
                    .WithDescription("表示するメッセージ")
            .AddFunction("get_message")
                .WithDescription("表示しているメッセージを取得します。")
            .GetTools();

        var invoker = new FunctionInvoker();
        invoker.Register<DisplayMessageParameters>("display_message", DisplayMessage);
        invoker.Register<GetMessageParameters>("get_message", GetMessage);

        var messages = new List<Message> { StringMessage.CreateUser("画面のメッセージの末尾に、こんにちはって追加して。") };

        var request = Request.Define()
            .WithGpt4Turbo()
            .WithMaxTokens(3000)
            .WithTemperature(0)
            .Create(messages, tools);

        var res = await _client.CompletionsStreamAsync(request, chunk => Debug.WriteLine(chunk));
        var fms = await invoker.InvokeAsync(res!.Choices[0]);

        request.Messages = [.. messages, res.Choices[0].Message!, .. fms];
        res = await _client.CompletionsStreamAsync(request, chunk => Debug.WriteLine(chunk));
    }

    private class DisplayMessageParameters
    {
        [JsonPropertyName("message")]
        public required string Message { get; set; }
    }

    private static Task<string> DisplayMessage(DisplayMessageParameters parameters)
    {
        return Task.FromResult("ok");
    }

    private class GetMessageParameters { }

    private static Task<string> GetMessage(GetMessageParameters parameters)
    {
        return Task.FromResult("へい！");
    }
}
