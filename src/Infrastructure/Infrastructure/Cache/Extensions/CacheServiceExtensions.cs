using Infrastructure.Cache.Factory;
using Infrastructure.Cache.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Cache.Extensions;

/// <summary>
/// Cache servislerinin registrasyonu için extension methods
/// </summary>
public static class CacheServiceExtensions
{
    /// <summary>
    /// Cache servislerini DI container'a register eder
    /// </summary>
    public static IServiceCollection AddCacheServices(
        this IServiceCollection services,
        Action<CacheSettings> configureOptions)
    {
        // Settings'i configure et
        services.Configure(configureOptions);

        // Memory Cache'i register et
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = 1024; // Default 1GB
        });

        // Factory ve Manager'ı register et
        services.AddSingleton<ICacheStrategyFactory, CacheStrategyFactory>();
        services.AddSingleton<ICacheManager, CacheManager>();

        return services;
    }
}