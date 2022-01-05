using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ShComp.DependencyInjection;

public static class Extensions
{
    public static void ConfigureUsingOptions<T>(this IServiceCollection services, IConfiguration configuration)
    {
        var parentType = typeof(T);
        if (parentType.GetNestedType("Options") is not { } optsType) throw new Exception();

        var configMethod = typeof(OptionsConfigurationServiceCollectionExtensions)
            .GetMethod("Configure", new[] { typeof(IServiceCollection), typeof(IConfiguration) })!
            .MakeGenericMethod(optsType);

        configMethod.Invoke(null, new object[] { services, configuration.GetSection(parentType.Name) });
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
