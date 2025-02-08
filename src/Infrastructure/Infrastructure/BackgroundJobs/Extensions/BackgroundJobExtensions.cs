using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.BackgroundJobs.Abstractions;
using Infrastructure.BackgroundJobs.Attributes;
using Infrastructure.BackgroundJobs.Filters;
using Infrastructure.BackgroundJobs.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.BackgroundJobs.Extensions;

/// <summary>
/// Background job servislerinin registrasyonu için extension methods
/// </summary>
public static class BackgroundJobExtensions
{
    /// <summary>
    /// Background job servislerini DI container'a register eder
    /// </summary>
    public static IServiceCollection AddBackgroundJobs(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Settings'i bind et
        var settings = new BackgroundJobSettings();
        configuration.GetSection("BackgroundJobs").Bind(settings);
        services.Configure<BackgroundJobSettings>(configuration.GetSection("BackgroundJobs"));

        // Hangfire servislerini ekle
        services.AddHangfire((provider, config) =>
        {
            // PostgreSQL storage yapılandırması
            config.UsePostgreSqlStorage(options =>
            {
                options.UseNpgsqlConnection(settings.Storage.ConnectionString);
            }, new PostgreSqlStorageOptions
            {
                SchemaName = settings.Storage.SchemaName,
                PrepareSchemaIfNecessary = true,
                QueuePollInterval = TimeSpan.FromSeconds(15),
                UseNativeDatabaseTransactions = true,
            });

            // Job filter'ları
            config.UseFilter(new AutomaticRetryAttribute { Attempts = 3 });
            config.UseFilter(new BackgroundJobAttribute());
        });

        // Background job server'ı ekle
        services.AddHangfireServer(options =>
        {
            options.WorkerCount = settings.WorkerCount;
            options.Queues = settings.Queues.OrderByDescending(x => x.Value)
                                          .Select(x => x.Key)
                                          .ToArray();
        });

        // Background job service'i register et
        services.AddScoped<IBackgroundJobService, BackgroundJobService>();

        return services;
    }

    /// <summary>
    /// Hangfire dashboard'ını yapılandırır
    /// </summary>
    public static IApplicationBuilder UseBackgroundJobDashboard(
        this IApplicationBuilder app,
        IConfiguration configuration)
    {
        var settings = new BackgroundJobSettings();
        configuration.GetSection("BackgroundJobs").Bind(settings);

        var options = new DashboardOptions
        {
            Authorization = new[]
            {
                new HangfireDashboardAuthFilter(
                    Options.Create(settings))
            },
            DashboardTitle = "Background Jobs",
            StatsPollingInterval = 5000,
            DisplayStorageConnectionString = false
        };

        app.UseHangfireDashboard(settings.DashboardPath, options);

        return app;
    }

    /// <summary>
    /// Örnek recurring job'ları ekler
    /// Development ortamında kullanışlı
    /// </summary>
    public static IApplicationBuilder UseBackgroundJobSeeder(this IApplicationBuilder app)
    {
        var jobService = app.ApplicationServices.GetRequiredService<IBackgroundJobService>();

        // Örnek: Her gün gece yarısı çalışacak bir job
        // jobService.RecurringJob<IExampleJob>(
        //     "daily-cleanup",
        //     job => job.CleanupAsync(),
        //     "0 0 * * *",
        //     "low");

        // Örnek: Her saatte bir çalışacak bir job
        // jobService.RecurringJob<IExampleJob>(
        //     "hourly-check",
        //     job => job.HealthCheckAsync(),
        //     "0 * * * *");

        return app;
    }
}