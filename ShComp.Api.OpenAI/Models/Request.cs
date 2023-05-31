using System.Text.Json.Serialization;

namespace ShComp.Api.OpenAI.Models;

public class Request : IWithModel, IWithMaxTokens, IWithTemperature, IRequestCreatable
{
    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; }

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("messages")]
    public IList<Message> Messages { get; set; }

    [JsonPropertyName("stream")]
    public bool Stream { get; set; }

#pragma warning disable CS8618
    private Request() { }
#pragma warning restore CS8618

    public Request(string model, int maxTokens, double temperature, IList<Message> messages)
    {
        Model = model;
        MaxTokens = maxTokens;
        Temperature = temperature;
        Messages = messages;
    }

    public static Request Create(IList<Message> messages) => new("gpt-3.5-turbo", 1000, 1, messages);

    public static IWithModel Define()
    {
        return new Request();
    }

    IWithMaxTokens IWithModel.WithGpt3_5Turbo()
    {
        Model = "gpt-3.5-turbo";
        return this;
    }

    IWithTemperature IWithMaxTokens.WithMaxTokens(int maxTokens)
    {
        MaxTokens = maxTokens;
        return this;
    }

    IRequestCreatable IWithTemperature.WithTemperature(double temperature)
    {
        Temperature = temperature;
        return this;
    }

    Request IRequestCreatable.Create(IList<Message> messages)
    {
        Messages = messages;
        return this;
    }
}

public interface IWithModel
{
    IWithMaxTokens WithGpt3_5Turbo();
}

public interface IWithMaxTokens
{
    IWithTemperature WithMaxTokens(int maxTokens);
}

public interface IWithTemperature
{
    IRequestCreatable WithTemperature(double temperature);
}

public interface IRequestCreatable
{
    Request Create(IList<Message> messages);
}
