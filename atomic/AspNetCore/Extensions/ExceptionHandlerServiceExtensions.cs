using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;

namespace Microsoft.Extensions.DependencyInjection;

public static class ExceptionHandlerServiceExtensions
{
    public static IServiceCollection AddExceptionHandler(this IServiceCollection services)
        => services.ConfigureGlobalResult().AddExceptionHandler<GlobalExceptionHandler>();

    public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder applicationBuilder)
        => applicationBuilder.UseExceptionHandler("/");
}
