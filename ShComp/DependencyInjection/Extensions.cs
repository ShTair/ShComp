using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace ShComp.DependencyInjection;

public static class Extensions
{
    public static void ConfigureUsingOptions<T>(this IServiceCollection services)
    {
        var configureMethod = typeof(Extensions).GetMethod(
            nameof(Configure),
            BindingFlags.Static | BindingFlags.NonPublic,
            new[] { typeof(IServiceCollection), typeof(string) })!;

        var parentType = typeof(T);
        var types = parentType.GetNestedTypes();

        services.AddOptions();

        foreach (var type in types)
        {
            string? sectionName = null;
            if (type.GetCustomAttribute<OptionsAttribute>() is { } optionsAttribute)
            {
                sectionName = optionsAttribute.SectionName ?? parentType.Name;
            }
            else if (type.Name == "Options")
            {
                sectionName = parentType.Name;
            }

            if (sectionName is null) continue;

            var bound = configureMethod.MakeGenericMethod(type);
            bound.Invoke(null, new object[] { services, sectionName });
        }
    }

    private static void Configure<TOptions>(IServiceCollection services, string sectionName) where TOptions : class
    {
        services.AddSingleton<IOptionsChangeTokenSource<TOptions>>(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            return new ConfigurationChangeTokenSource<TOptions>(Options.DefaultName, config.GetSection(sectionName));
        });

        services.AddSingleton<IConfigureOptions<TOptions>>(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            return new NamedConfigureFromConfigurationOptions<TOptions>(Options.DefaultName, config.GetSection(sectionName), delegate { });
        });
    }

    public static void AddSingletonAndConfigure<T>(this IServiceCollection services) where T : class
    {
        services.ConfigureUsingOptions<T>();
        services.AddSingleton<T>();
    }

    public static void AddHostedServiceAndConfigure<T>(this IServiceCollection services) where T : class, IHostedService
    {
        services.ConfigureUsingOptions<T>();
        services.AddHostedService<T>();
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class OptionsAttribute : Attribute
{
    public string? SectionName { get; set; }
}
