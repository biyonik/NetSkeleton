using Infrastructure.Logging.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Infrastructure.Logging.Enrichers;

/// <summary>
/// Log yapılandırma builder'ı
/// </summary>
public static class LogConfigurator
{
    /// <summary>
    /// Serilog yapılandırmasını oluşturur
    /// </summary>
    public static Logger CreateLogger(
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        var settings = configuration.GetSection("Logging").Get<LogSettings>() ?? new LogSettings();
        var logLevel = ParseLogLevel(settings.MinimumLevel);

        // Temel yapılandırma
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .Enrich.FromLogContext();

        // Custom enricher ekle
        loggerConfiguration.Enrich.With(new CustomLogEnricher(
            httpContextAccessor,
            settings.Enricher.ApplicationName,
            settings.Enricher.Environment));

        // Machine name ve thread ID ekle
        if (settings.Enricher.IncludeMachineName)
            loggerConfiguration.Enrich.WithMachineName();
        
        if (settings.Enricher.IncludeThreadId)
            loggerConfiguration.Enrich.WithThreadId();

        // Sink'leri yapılandır
        ConfigureSinks(loggerConfiguration, settings);

        return loggerConfiguration.CreateLogger();
    }

    /// <summary>
    /// Sink'leri yapılandırır
    /// </summary>
    private static void ConfigureSinks(LoggerConfiguration loggerConfiguration, LogSettings settings)
    {
        // Console sink
        if (settings.WriteToConsole)
        {
            loggerConfiguration.WriteTo.Async(a => a.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"));
        }

        // File sink
        if (settings.File != null)
        {
            var rollingInterval = Enum.Parse<RollingInterval>(settings.File.RollingInterval);
        
            loggerConfiguration.WriteTo.Async(a => a.File(
                path: settings.File.Path,
                rollingInterval: rollingInterval,
                fileSizeLimitBytes: settings.File.FileSizeLimitBytes,
                retainedFileCountLimit: settings.File.RetainedFileCountLimit,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} " +
                                "{Properties:j}{NewLine}{Exception}"));
        }

        // Elastic Search sink
        if (settings.ElasticSearch != null)
        {
            var elasticOptions = new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri(settings.ElasticSearch.Url))
            {
                IndexFormat = settings.ElasticSearch.IndexFormat,
                BatchPostingLimit = settings.ElasticSearch.BatchPostingLimit,
                Period = TimeSpan.FromSeconds(settings.ElasticSearch.Period),
                AutoRegisterTemplate = true,
                TemplateName = "serilog-logs"
            };

            loggerConfiguration.WriteTo.Async(a => a.Elasticsearch(elasticOptions));
        }
    }

    /// <summary>
    /// Log seviyesini parse eder
    /// </summary>
    private static LogEventLevel ParseLogLevel(string level)
    {
        return level?.ToLower() switch
        {
            "verbose" => LogEventLevel.Verbose,
            "debug" => LogEventLevel.Debug,
            "information" => LogEventLevel.Information,
            "warning" => LogEventLevel.Warning,
            "error" => LogEventLevel.Error,
            "fatal" => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };
    }
}