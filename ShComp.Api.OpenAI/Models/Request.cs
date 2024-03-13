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

    [JsonPropertyName("tools")]
    public IList<Tool>? Tools { get; set; }

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

    public static Request Create(IList<Message> messages) => new(GptModels.Gpt3_5Turbo, 1000, 1, messages);

    public static IWithModel Define()
    {
        return new Request();
    }

    IWithMaxTokens IWithModel.WithGpt3_5Turbo()
    {
        Model = GptModels.Gpt3_5Turbo;
        return this;
    }

    IWithMaxTokens IWithModel.WithGpt4()
    {
        Model = GptModels.Gpt4;
        return this;
    }

    IWithMaxTokens IWithModel.WithGpt4Turbo()
    {
        Model = GptModels.Gpt4Turbo;
        return this;
    }

    IWithMaxTokens IWithModel.WithGpt4TurboWithVision()
    {
        Model = GptModels.Gpt4TurboWithVision;
        return this;
    }

    IWithMaxTokens IWithModel.With(string modelName)
    {
        Model = modelName;
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

    Request IRequestCreatable.Create(IList<Message> messages, IList<Tool>? tools)
    {
        Messages = messages;
        Tools = tools;
        return this;
    }
}

public interface IWithModel
{
    IWithMaxTokens WithGpt3_5Turbo();

    IWithMaxTokens WithGpt4();

    IWithMaxTokens WithGpt4Turbo();

    IWithMaxTokens WithGpt4TurboWithVision();

    IWithMaxTokens With(string modelName);
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
    Request Create(IList<Message> messages, IList<Tool>? tools = null);
}
