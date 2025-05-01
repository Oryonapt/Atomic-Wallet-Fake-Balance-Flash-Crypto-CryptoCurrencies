namespace Microsoft.Extensions.DependencyInjection;

public static class LazyLoadingServiceExtensions
{
    public static IServiceCollection AddLazyLoadingAccessor(this IServiceCollection services)
        => services.AddTransient(typeof(ILazyLoading<>), typeof(LazyLoading<>));
}
