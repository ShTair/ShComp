using ShComp.Api.OpenAI.Models;
using System.Text.Json;

namespace ShComp.Api.OpenAI;

public class FunctionInvoker
{
    private readonly Dictionary<string, Func<string, Task<string>>> _functions = [];

    public void Register<T>(string name, Func<T, Task<string>> function)
    {
        _functions.Add(name, async (parameters) =>
        {
            var parameter = JsonSerializer.Deserialize<T>(parameters);
            ArgumentNullException.ThrowIfNull(parameter);
            return await function(parameter);
        });
    }

    public async Task<List<Message>> InvokeAsync(Choice choice)
    {
        List<Message> messages = [];
        if (choice is { Message.ToolCalls: { } toolCalls })
        {
            foreach (var toolCall in toolCalls)
            {
                var function = toolCall.Function;
                var result = await _functions[function.Name](function.Arguments);

                var message = Message.CreateTool(result);
                message.ToolCallId = toolCall.Id;
                messages.Add(message);
            }
        }

        return messages;
    }
}
