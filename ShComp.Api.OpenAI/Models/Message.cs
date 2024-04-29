using System.Text.Json.Serialization;

namespace ShComp.Api.OpenAI.Models;

[JsonDerivedType(typeof(StringMessage))]
[JsonDerivedType(typeof(ArrayMessage))]
public abstract class Message
{
    [JsonPropertyName("role")]
    public string Role { get; set; }

    protected Message(string role)
    {
        Role = role;
    }
}

public class StringMessage : Message
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("tool_calls")]
    public List<ToolCall>? ToolCalls { get; set; }

    [JsonPropertyName("tool_call_id")]
    public string? ToolCallId { get; set; }

    public StringMessage(string role) : base(role) { }


    public static StringMessage CreateSystem(string content) => new(MessageRoleTypes.System) { Content = content };

    public static StringMessage CreateUser(string content) => new(MessageRoleTypes.User) { Content = content };

    public static StringMessage CreateAssistant(string content) => new(MessageRoleTypes.Assistant) { Content = content };

    public static StringMessage CreateTool(string content, string toolCallId) => new(MessageRoleTypes.Tool) { Content = content, ToolCallId = toolCallId };
}


public class ArrayMessage : Message
{
    [JsonPropertyName("content")]
    public List<IMessageContent> Contents { get; set; }

    public ArrayMessage(string role, List<IMessageContent> contents) : base(role)
    {
        Contents = contents;
    }
}




//public abstract class UserMessage : Message
//{
//    public UserMessage() : base("user") { }
//}

//public class StringUserMessage : UserMessage
//{
//    [JsonPropertyName("content")]
//    public string Content { get; set; }

//    public StringUserMessage(string content)
//    {
//        Content = content;
//    }
//}

//public class ArrayUserMessage : UserMessage
//{
//    [JsonPropertyName("content")]
//    public List<IMessageContent> Contents { get; set; }

//    public ArrayUserMessage(List<IMessageContent> contents)
//    {
//        Contents = contents;
//    }
//}

//public abstract class StringMessage : Message
//{
//    [JsonPropertyName("content")]
//    public string Content { get; set; }

//    protected StringMessage(string role, string content) : base(role)
//    {
//        Content = content;
//    }

//    public override string ToString() => $"{Role}: {Content}";
//}

//public class SystemMessage : StringMessage
//{
//    public SystemMessage(string content) : base("system", content) { }
//}

//public class AssistantMessage : Message
//{
//    [JsonPropertyName("content")]
//    public string? Content { get; set; }

//    [JsonPropertyName("tool_calls")]
//    public List<ToolCall>? ToolCalls { get; set; }

//    public AssistantMessage(string content) : base("assistant")
//    {
//        Content = content;
//    }
//}

//public class ToolMessage : StringMessage
//{
//    [JsonPropertyName("tool_call_id")]
//    public string ToolCallId { get; set; }

//    public ToolMessage(string content, string toolCallId) : base("tool", content)
//    {
//        ToolCallId = toolCallId;
//    }
//}
