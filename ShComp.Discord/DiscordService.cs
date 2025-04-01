using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace ShComp.Discord;

public class DiscordService
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private readonly IOptions<Options> _options;
    private readonly DiscordSocketConfig _config;

    public DiscordService(IOptions<Options> options)
    {
        _options = options;
        _config = new DiscordSocketConfig { GatewayIntents = GatewayIntents.Guilds };
    }

    public Task<ulong> SendMessageAsync(ulong channelId, string? text)
    {
        return CommandChannelAsync(channelId, async channel => (await channel.SendMessageAsync(text)).Id);
    }

    public Task<ulong> SendFileAsync(ulong channelId, Stream stream, string fileName, string? text = null)
    {
        return CommandChannelAsync(channelId, async channel => (await channel.SendFileAsync(stream, fileName, text)).Id);
    }

    public Task AttachFileAsync(ulong channelId, ulong messageId, Stream stream, string fileName)
    {
        return CommandChannelAsync(channelId, async channel =>
        {
            var message = await channel.GetMessageAsync(messageId) as RestUserMessage;
            await message!.ModifyAsync(props =>
            {
                props.Attachments = new Optional<IEnumerable<FileAttachment>>([
                    ..props.Attachments.IsSpecified ? props.Attachments.Value : [],
                    new FileAttachment(stream, fileName)
                ]);
            });
            return messageId;
        });
    }

    private async Task<T> CommandAsync<T>(Func<DiscordSocketClient, Task<T>> func)
    {
        try
        {
            await _semaphore.WaitAsync();

            await using var client = new DiscordSocketClient(_config);
            client.Log += LogAsync;

            var tcs = new TaskCompletionSource();
            client.Ready += () =>
            {
                Task.Run(tcs.TrySetResult);
                return Task.CompletedTask;
            };
            _ = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                tcs.TrySetException(new TimeoutException());
            });

            await client.LoginAsync(TokenType.Bot, _options.Value.Token);
            await client.StartAsync();

            await tcs.Task;
            return await func(client);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<T> CommandChannelAsync<T>(ulong channelId, Func<IMessageChannel, Task<T>> func)
    {
        return await CommandAsync(async client =>
        {
            var channel = await client.GetChannelAsync(channelId) as IMessageChannel;
            return await func(channel!);
        });
    }

    private Task LogAsync(LogMessage message)
    {
        Console.WriteLine($"{message.Severity} {message.Source} {message.Message}");
        if (message.Exception is { } ex) Console.WriteLine(ex);
        return Task.CompletedTask;
    }

    public class Options
    {
        public string Token { get; set; } = default!;
    }
}
