using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Microsoft.Extensions.DependencyInjection;

public static class MvcOptionsServiceExensions
{
    public static IServiceCollection ConfigureMvcOptions(this IServiceCollection services)
        => services.ConfigureGlobalResult().Configure<MvcOptions>(options =>
        {
            options.Filters.Add<GlobalResultFilterAttribute>();
            options.Filters.Add<GlobalExceptionFilterAttribute>();
            options.ModelMetadataDetailsProviders.Add(new ValidationMetadataLocalizationProvider());
        });
}