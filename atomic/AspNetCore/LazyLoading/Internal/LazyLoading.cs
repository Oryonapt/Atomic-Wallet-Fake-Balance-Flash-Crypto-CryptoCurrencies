namespace Microsoft.Extensions.DependencyInjection;

public class LazyLoading<T>(IServiceProvider serviceProvider) : Lazy<T>(serviceProvider.GetRequiredService<T>), ILazyLoading<T> where T : notnull;