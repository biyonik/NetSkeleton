using Infrastructure.Logging.Enrichers;
using Infrastructure.Logging.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Infrastructure.Logging.Extensions;

/// <summary>
/// Logging servislerinin registrasyonu için extension methods
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Logging servislerini DI container'a register eder
    /// </summary>
    public static IServiceCollection AddCustomLogging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // HTTP context accessor ekle
        services.AddHttpContextAccessor();

        // Logger'ı oluştur ve register et
        var httpContextAccessor = services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>();
        var logger = LogConfigurator.CreateLogger(configuration, httpContextAccessor);

        services.AddSingleton(logger);
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(logger, dispose: true);
        });

        return services;
    }

    /// <summary>
    /// Request/response logging middleware'ini ekler
    /// </summary>
    public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}