namespace Microsoft.Extensions.Configuration;

public static class ConfigurationBinderExtensions
{
    public static T Bind<T>(this IConfiguration configuration) where T : class, new()
    {
        var objectinstace = new T();
        configuration.Bind(objectinstace);
        return objectinstace;
    }

    public static T Bind<T>(this IConfiguration configuration, string key) where T : class, new()
    {
        var objectinstace = new T();
        configuration.Bind(key, objectinstace);
        return objectinstace;
    }

    public static T Bind<T>(this IConfiguration configuration, Action<BinderOptions>? configureOptions) where T : class, new()
    {
        var objectinstace = new T();
        configuration.Bind(objectinstace, configureOptions);
        return objectinstace;
    }
}