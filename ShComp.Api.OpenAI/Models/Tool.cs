namespace ShComp.Api.OpenAI.Models;

public class Tool
{
    public required string Type { get; set; }

    public required Function Function { get; set; }
}

public static class ToolTypes
{
    public const string Function = "function";
}

public class Function
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    public required Parameters Parameters { get; set; }
}

public class Parameters
{
    public required string Type { get; set; }

    public required Dictionary<string, Property> Properties { get; set; }

    public required List<string> Required { get; set; }
}

public static class ParametersTypes
{
    public const string Object = "object";
}

public class Property
{
    public required string Type { get; set; }

    public List<string>? Enum { get; set; }

    public string? Description { get; set; }
}

public static class PropertyTypes
{
    public const string String = "string";

    public const string Integer = "integer";
}

public class Tools : IBlankTools, IWithFunctionDescription, IAddedFunction, IAddFirstEnumValue, IAddedEnumValue, IAddedProperty
{
    private readonly List<Tool> _tools;

    private Tool _currentTool = default!;

    private Property _currentProperty = default!;

    private Tools() { _tools = []; }

    public static IBlankTools Define() { return new Tools(); }

    IWithFunctionDescription IAddFunction.AddFunction(string name)
    {
        _currentTool = new Tool
        {
            Type = ToolTypes.Function,
            Function = new Function
            {
                Name = name,
                Parameters = new Parameters
                {
                    Type = ParametersTypes.Object,
                    Properties = [],
                    Required = [],
                },
            }
        };

        _tools.Add(_currentTool);

        return this;
    }

    IAddedFunction IWithFunctionDescription.WithDescription(string description)
    {
        _currentTool.Function.Description = description;

        return this;
    }

    IWithPropertyDescription IAddProperty.AddStringProperty(string name, bool required)
    {
        _currentProperty = new Property { Type = PropertyTypes.String };
        _currentTool.Function.Parameters.Properties.Add(name, _currentProperty);

        if (required) _currentTool.Function.Parameters.Required.Add(name);

        return this;
    }

    IWithPropertyDescription IAddProperty.AddIntegerProperty(string name, bool required)
    {
        _currentProperty = new Property { Type = PropertyTypes.Integer };
        _currentTool.Function.Parameters.Properties.Add(name, _currentProperty);

        if (required) _currentTool.Function.Parameters.Required.Add(name);

        return this;
    }

    IAddFirstEnumValue IAddProperty.AddEnumProperty(string name, bool required)
    {
        _currentProperty = new Property { Type = PropertyTypes.String, Enum = [] };
        _currentTool.Function.Parameters.Properties.Add(name, _currentProperty);

        if (required) _currentTool.Function.Parameters.Required.Add(name);

        return this;
    }

    IAddedEnumValue IAddEnumValue.AddEnumValue(string value)
    {
        _currentProperty.Enum!.Add(value);

        return this;
    }

    IAddedProperty IWithPropertyDescription.WithDescription(string description)
    {
        _currentProperty.Description = description;

        return this;
    }

    List<Tool> IAddedProperty.GetTools()
    {
        return _tools;
    }
}

public interface IBlankTools : IAddFunction { }

public interface IAddFunction
{
    IWithFunctionDescription AddFunction(string name);
}

public interface IWithFunctionDescription : IAddProperty, IAddedProperty
{
    IAddedFunction WithDescription(string description);
}

public interface IAddedFunction : IAddProperty, IAddedProperty { }

public interface IAddProperty
{
    IWithPropertyDescription AddStringProperty(string name, bool required = false);

    IWithPropertyDescription AddIntegerProperty(string name, bool required = false);

    IAddFirstEnumValue AddEnumProperty(string name, bool required = false);
}

public interface IAddFirstEnumValue : IAddEnumValue { }

public interface IAddEnumValue
{
    IAddedEnumValue AddEnumValue(string value);
}

public interface IAddedEnumValue : IAddEnumValue, IWithPropertyDescription { }

public interface IWithPropertyDescription : IAddedProperty
{
    IAddedProperty WithDescription(string description);
}

public interface IAddedProperty : IAddProperty, IAddFunction
{
    List<Tool> GetTools();
}
