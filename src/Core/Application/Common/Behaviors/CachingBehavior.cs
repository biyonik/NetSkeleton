using Application.Common.CQRS;
using Infrastructure.Cache;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

/// <summary>
/// Caching pipeline behavior'u
/// </summary>
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableQuery<TResponse>
{
    private readonly ICacheManager _cacheManager;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(
        ICacheManager cacheManager,
        ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cacheManager = cacheManager;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var cacheKey = request.CacheKey;
        var cachedResponse = await _cacheManager.GetAsync<TResponse>(cacheKey, cancellationToken);

        if (cachedResponse != null)
        {
            _logger.LogInformation("Cache hit for {CacheKey}", cacheKey);
            return cachedResponse;
        }

        _logger.LogInformation("Cache miss for {CacheKey}", cacheKey);
        var response = await next();

        if (response != null)
        {
            await _cacheManager.SetAsync(
                cacheKey,
                response,
                request.CacheExpiration,
                cancellationToken);
        }

        return response;
    }
}