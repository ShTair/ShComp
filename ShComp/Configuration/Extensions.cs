using ShComp.Configuration;

namespace Microsoft.Extensions.Configuration;

public static class Extensions
{
    public static IConfigurationBuilder AddStringArray(this IConfigurationBuilder builder, IEnumerable<string> keys, IEnumerable<string> values)
    {
        return builder.Add(new StringArrayConfigurationSource(keys, values));
    }
}
