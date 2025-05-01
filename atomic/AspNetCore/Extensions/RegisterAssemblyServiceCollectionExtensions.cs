using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class RegisterAssemblyServiceCollectionExtensions
{
    private static readonly IReadOnlyCollection<Type> _dependencyMapping = [typeof(IScoped), typeof(ISingleton), typeof(ITransient)];

    public static IServiceCollection RegisterServiceFromAssembly(this IServiceCollection services, string assemblyName)
        => services.RegisterServiceFromAssembly(Assembly.Load(assemblyName));

    public static IServiceCollection TryRegisterServiceFromAssembly(this IServiceCollection services, string assemblyName)
        => services.TryRegisterServiceFromAssembly(Assembly.Load(assemblyName));

    public static IServiceCollection RegisterServiceFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        assembly.GetTypes().Where(w => w.IsInterface && _dependencyMapping.Any(a => a.IsAssignableFrom(w))).ForEach(serviceType =>
        {
            assembly.GetTypes().Where(w => w.IsClass && serviceType.IsAssignableFrom(w)).ForEach(implementationType =>
            {
                ServiceLifetime lifetime = ServiceLifetime.Scoped;
                if (typeof(IScoped).IsAssignableFrom(serviceType))
                {
                    lifetime = ServiceLifetime.Scoped;
                }
                else if (typeof(ISingleton).IsAssignableFrom(serviceType))
                {
                    lifetime = ServiceLifetime.Singleton;
                }
                else if (typeof(ITransient).IsAssignableFrom(serviceType))
                {
                    lifetime = ServiceLifetime.Transient;
                }

                if (implementationType.GetCustomAttribute<DependencyAttribute>() is not DependencyAttribute dependency)
                {
                    services.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));
                }
                else
                {
                    services.Add(new ServiceDescriptor(serviceType, dependency?.StoredKey, implementationType, lifetime!));
                }
            });
        });

        return services;
    }

    public static IServiceCollection TryRegisterServiceFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        assembly.GetTypes().Where(w => w.IsInterface && _dependencyMapping.Any(a => a.IsAssignableFrom(w))).ForEach(serviceType =>
        {
            assembly.GetTypes().Where(w => w.IsClass && serviceType.IsAssignableFrom(w)).ForEach(implementationType =>
            {
                ServiceLifetime lifetime = ServiceLifetime.Scoped;
                if (typeof(IScoped).IsAssignableFrom(serviceType))
                {
                    lifetime = ServiceLifetime.Scoped;
                }
                else if (typeof(ISingleton).IsAssignableFrom(serviceType))
                {
                    lifetime = ServiceLifetime.Singleton;
                }
                else if (typeof(ITransient).IsAssignableFrom(serviceType))
                {
                    lifetime = ServiceLifetime.Transient;
                }

                if (implementationType.GetCustomAttribute<DependencyAttribute>() is not DependencyAttribute dependency)
                {
                    services.TryAdd(new ServiceDescriptor(serviceType, implementationType, lifetime));
                }
                else
                {
                    services.TryAdd(new ServiceDescriptor(serviceType, dependency?.StoredKey, implementationType, lifetime!));
                }
            });
        });

        return services;
    }
}