using System.Text.Json;

namespace Microsoft.Extensions.DependencyInjection;

public static class JsonOptionsServiceExtensions
{
    public static IServiceCollection ConfigureHttpJsonOptions(this IServiceCollection services)
        => services.Configure<AspNetCore.Http.Json.JsonOptions>(configureOptions => configureOptions.SerializerOptions.ApplyWebDefault());

    public static IServiceCollection ConfigureMvcJsonOptions(this IServiceCollection services)
        => services.Configure<AspNetCore.Mvc.JsonOptions>(configureOptions => configureOptions.JsonSerializerOptions.ApplyWebDefault());
}
