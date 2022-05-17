using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace ShComp.DependencyInjection;

public static class Extensions
{
    public static void ConfigureUsingOptions<T>(this IServiceCollection services, IConfiguration configuration)
    {
        var configMethod = typeof(OptionsConfigurationServiceCollectionExtensions)
            .GetMethod("Configure", new[] { typeof(IServiceCollection), typeof(IConfiguration) })!;

        var parentType = typeof(T);
        var types = parentType.GetNestedTypes();

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

            var genericConfigMethod = configMethod.MakeGenericMethod(type);
            genericConfigMethod.Invoke(null, new object[] { services, configuration.GetSection(sectionName) });
        }
    }

    public static void AddSingletonWithConfiguration<T>(this IServiceCollection services, IConfiguration configuration) where T : class
    {
        services.ConfigureUsingOptions<T>(configuration);
        services.AddSingleton<T>();
    }

    public static void AddHostedServiceWithConfiguration<T>(this IServiceCollection services, IConfiguration configuration) where T : class, IHostedService
    {
        services.ConfigureUsingOptions<T>(configuration);
        services.AddHostedService<T>();
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class OptionsAttribute : Attribute
{
    public string? SectionName { get; set; }
}
