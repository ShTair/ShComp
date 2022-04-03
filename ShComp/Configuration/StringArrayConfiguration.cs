using Microsoft.Extensions.Configuration;

namespace ShComp.Configuration;

public class StringArrayConfigurationSource : IConfigurationSource
{
    private readonly IEnumerable<string> _keys;
    private readonly IEnumerable<string> _values;

    public StringArrayConfigurationSource(IEnumerable<string> keys, IEnumerable<string> values)
    {
        _keys = keys;
        _values = values;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new StringArrayConfigurationProvider(_keys, _values);
    }
}

public class StringArrayConfigurationProvider : ConfigurationProvider
{
    private readonly IEnumerable<string> _keys;
    private readonly IEnumerable<string> _values;

    public StringArrayConfigurationProvider(IEnumerable<string> keys, IEnumerable<string> values)
    {
        _keys = keys;
        _values = values;
    }

    public override void Load()
    {
        foreach (var (key, value) in _keys.Zip(_values, (key, value) => (key, value)))
        {
            Data[key] = value;
        }
    }
}
