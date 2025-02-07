using Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence.Context;
using Persistence.Repositories;

namespace Persistence.Extensions;

/// <summary>
/// Persistence katmanı için servis kayıt extension'ları
/// </summary>
public static class PersistenceServiceExtensions
{
    /// <summary>
    /// Persistence katmanı servislerini DI container'a register eder
    /// </summary>
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext'i register et
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => 
                {
                    // Migration assembly'yi belirt
                    b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    // Command timeout
                    b.CommandTimeout(30);
                    // Retry on failure
                    b.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                });

            // Query splitting behavior
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);

            // Detailed errors in development
            #if DEBUG
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
            #endif
        });

        // Generic repository'leri register et
        services.AddScoped(typeof(IRepository<,>), typeof(BaseRepository<,>));
        services.AddScoped(typeof(IReadRepository<,>), typeof(BaseRepository<,>));

        return services;
    }

    /// <summary>
    /// Development ortamı için gerekli database migration'ları yapar
    /// </summary>
    public static async Task MigrateDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }
    }

    /// <summary>
    /// Veritabanını siler ve yeniden oluşturur
    /// Sadece development ortamında kullanılmalıdır
    /// </summary>
    public static async Task RecreateCleanDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            // Veritabanını sil
            await context.Database.EnsureDeletedAsync();

            // Veritabanını oluştur ve migration'ları uygula
            await context.Database.MigrateAsync();

            // Seed data eklenebilir
            // await SeedDataAsync(context);
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
            logger.LogError(ex, "An error occurred while recreating the database.");
            throw;
        }
    }

    /// <summary>
    /// Health check için veritabanı bağlantısını kontrol eder
    /// </summary>
    public static async Task<bool> CheckDatabaseConnectionAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            // Basit bir sorgu ile bağlantıyı test et
            await context.Database.CanConnectAsync();
            return true;
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
            logger.LogError(ex, "Database connection check failed.");
            return false;
        }
    }
}