using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

/// <summary>
/// Logging pipeline behavior'u
/// </summary>
public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestType = typeof(TRequest).Name;
        var requestId = Guid.NewGuid().ToString();

        try
        {
            logger.LogInformation(
                "Starting request {RequestType} [{RequestId}]. Request: {@Request}",
                requestType, requestId, request);

            var timer = Stopwatch.StartNew();
            var response = await next();
            timer.Stop();

            logger.LogInformation(
                "Completed request {RequestType} [{RequestId}] in {ElapsedMilliseconds}ms",
                requestType, requestId, timer.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Request {RequestType} [{RequestId}] failed",
                requestType, requestId);
            throw;
        }
    }
}
