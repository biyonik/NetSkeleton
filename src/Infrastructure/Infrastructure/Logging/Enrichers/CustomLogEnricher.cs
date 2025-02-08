using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace Infrastructure.Logging.Enrichers;

/// <summary>
/// Custom log enricher
/// </summary>
public class CustomLogEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _applicationName;
    private readonly string _environment;

    public CustomLogEnricher(
        IHttpContextAccessor httpContextAccessor,
        string applicationName,
        string environment)
    {
        _httpContextAccessor = httpContextAccessor;
        _applicationName = applicationName;
        _environment = environment;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        // Uygulama adı
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
            "ApplicationName", _applicationName));

        // Ortam
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
            "Environment", _environment));

        // Correlation ID
        var correlationId = GetCorrelationId();
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
            "CorrelationId", correlationId));

        // Kullanıcı bilgileri
        if (_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                    "UserId", userId));
            }
        }

        // IP adresi
        var ipAddress = GetClientIpAddress();
        if (!string.IsNullOrEmpty(ipAddress))
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "ClientIp", ipAddress));
        }

        // Request path
        var path = _httpContextAccessor.HttpContext?.Request?.Path.Value;
        if (!string.IsNullOrEmpty(path))
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "RequestPath", path));
        }
    }

    private string GetCorrelationId()
    {
        if (_httpContextAccessor.HttpContext == null)
            return Guid.NewGuid().ToString();

        const string CorrelationIdHeader = "X-Correlation-ID";

        return _httpContextAccessor.HttpContext.Request?.Headers[CorrelationIdHeader].FirstOrDefault()
            ?? _httpContextAccessor.HttpContext.TraceIdentifier
            ?? Guid.NewGuid().ToString();
    }

    private string? GetClientIpAddress()
    {
        if (_httpContextAccessor.HttpContext == null)
            return null;

        return _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}