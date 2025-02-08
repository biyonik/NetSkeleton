using Infrastructure.FileStorage.Factory;
using Infrastructure.FileStorage.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.FileStorage.Extensions;

/// <summary>
/// Storage servislerinin registrasyonu için extension methods
/// </summary>
public static class StorageServiceExtensions
{
    /// <summary>
    /// Storage servislerini DI container'a register eder
    /// </summary>
    public static IServiceCollection AddStorageServices(
        this IServiceCollection services,
        Action<StorageSettings> configureOptions)
    {
        // Settings'i configure et
        services.Configure(configureOptions);

        // Factory ve Manager'ı register et
        services.AddSingleton<IStorageStrategyFactory, StorageStrategyFactory>();
        services.AddSingleton<IStorageManager, StorageManager>();

        return services;
    }
}