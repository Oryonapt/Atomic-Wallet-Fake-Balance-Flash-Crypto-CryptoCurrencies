using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.AspNetCore.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ApplyDefault(this WebApplication webApplication)
    {
        webApplication.UseRouting();
        webApplication.UsePathBase("/test");
        webApplication.UseExceptionHandler("/");

        webApplication.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All,
        });

        webApplication.UseCookiePolicy(new CookiePolicyOptions
        {
            HttpOnly = HttpOnlyPolicy.Always,
        });

        webApplication.UseCors(
            policy =>
                policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .DisallowCredentials()
                .SetIsOriginAllowed(t => true)
                .SetPreflightMaxAge(TimeSpan.FromMinutes(2))
                .SetIsOriginAllowedToAllowWildcardSubdomains()
        );
        webApplication.UseResponseCaching();
        webApplication.UseRequestDecompression();

        webApplication.UseStaticFiles();
        webApplication.UseHealthChecks("/health");

        webApplication.MapControllers();

        return webApplication;
    }

    public static WebApplicationBuilder ApplayDefault(this WebApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Environment.SetBasePath(AppContext.BaseDirectory);
        applicationBuilder.Configuration.SetBasePath(AppContext.BaseDirectory);

        applicationBuilder.Services.AddControllers();
        applicationBuilder.Services.AddHealthChecks();
        applicationBuilder.Services.AddResponseCaching();
        applicationBuilder.Services.ConfigureMvcOptions();
        applicationBuilder.Services.AddExceptionHandler();
        applicationBuilder.Services.AddLazyLoadingAccessor();
        applicationBuilder.Services.AddHttpContextAccessor();
        applicationBuilder.Services.AddRequestDecompression();
        applicationBuilder.Services.AddEndpointsApiExplorer();
        applicationBuilder.Services.ConfigureMvcJsonOptions();
        applicationBuilder.Services.ConfigureHttpJsonOptions();
        applicationBuilder.Services.ConfigureApiBehaviorInvalidModelStateResponse();

        return applicationBuilder;
    }
}