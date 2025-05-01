using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System.ComponentModel;

namespace Microsoft.EntityFrameworkCore;

public static class EntityFrameworkCoreServiceCollectionExtensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IServiceCollection AddEntityFrameworkCoreServices(this IServiceCollection serviceCollection)
    {
        new EntityFrameworkRelationalServicesBuilder(serviceCollection)
            .TryAdd<IConventionSetPlugin, EntityFrameworkCoreConventionSetPlugin>()
            .TryAdd<IRelationalTypeMappingSourcePlugin, EntityFrameworkCoreRelationalTypeMappingSourcePlugin>()
            .TryAdd<ISingletonOptions, IEntityFrameworkCoreSingletonOptions>(p => p.GetRequiredService<IEntityFrameworkCoreSingletonOptions>())
            .TryAddProviderSpecificServices(
                b => b
                    .TryAddSingleton<IEntityFrameworkCoreSingletonOptions, EntityFrameworkCoreSingletonOptions>());

        return serviceCollection;
    }
}
