using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Logging.Middleware;

/// <summary>
/// Request/response detaylarını loglayan middleware
/// </summary>
public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    private readonly HashSet<string> _excludePaths;

    public RequestResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;

        // Log'lanmayacak path'ler
        _excludePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "/health",
            "/metrics",
            "/favicon.ico"
        };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Health check gibi endpoint'leri loglama
        if (ShouldSkipLogging(context))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var requestBody = await GetRequestBodyAsync(context);

        try
        {
            // Request'i logla
            LogRequest(context, requestBody);

            // Response body'yi yakalamak için
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            // Pipeline'ı çalıştır
            await _next(context);

            // Response'u logla
            await LogResponseAsync(context, stopwatch, responseBody);

            // Orijinal response body'yi geri yaz
            await WriteResponseBodyAsync(responseBody, originalBodyStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing request");
            throw;
        }
    }

    private bool ShouldSkipLogging(HttpContext context)
    {
        return _excludePaths.Contains(context.Request.Path);
    }

    private async Task<string> GetRequestBodyAsync(HttpContext context)
    {
        if (!context.Request.Body.CanRead || !context.Request.Body.CanSeek)
            return string.Empty;

        context.Request.EnableBuffering();

        var buffer = new byte[context.Request.ContentLength ?? 0];
        await context.Request.Body.ReadAsync(buffer);
        var requestBody = Encoding.UTF8.GetString(buffer);

        context.Request.Body.Position = 0;
        return requestBody;
    }

    private void LogRequest(HttpContext context, string requestBody)
    {
        var request = context.Request;

        _logger.LogInformation(
            "HTTP Request: {Method} {Scheme}://{Host}{Path}{QueryString}\n" +
            "Headers: {@Headers}\n" +
            "Body: {Body}",
            request.Method,
            request.Scheme,
            request.Host,
            request.Path,
            request.QueryString,
            request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
            requestBody);
    }

    private async Task LogResponseAsync(HttpContext context, Stopwatch stopwatch, MemoryStream responseBody)
    {
        responseBody.Position = 0;
        var responseBodyText = await new StreamReader(responseBody).ReadToEndAsync();
        responseBody.Position = 0;

        var response = context.Response;
        var elapsed = stopwatch.ElapsedMilliseconds;

        _logger.LogInformation(
            "HTTP Response: {StatusCode} in {ElapsedMs}ms\n" +
            "Headers: {@Headers}\n" +
            "Body: {Body}",
            response.StatusCode,
            elapsed,
            response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
            responseBodyText);
    }

    private async Task WriteResponseBodyAsync(MemoryStream responseBody, Stream originalBodyStream)
    {
        responseBody.Position = 0;
        await responseBody.CopyToAsync(originalBodyStream);
    }
}