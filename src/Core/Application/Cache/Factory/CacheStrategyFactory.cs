using Application.Cache.Abstractions;
using Application.Cache.Settings;
using Application.Cache.Strategies;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Application.Cache.Factory;

/// <summary>
/// Cache factory implementasyonu
/// </summary>
public class CacheStrategyFactory : ICacheStrategyFactory
{
    private readonly IOptions<CacheSettings> _settings;
    private readonly IMemoryCache _memoryCache;

    public CacheStrategyFactory(
        IOptions<CacheSettings> settings,
        IMemoryCache memoryCache)
    {
        _settings = settings;
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// Verilen strateji tipine göre cache implementasyonu oluşturur
    /// </summary>
    public ICacheStrategy CreateStrategy(CacheStrategy strategy)
    {
        return strategy switch
        {
            CacheStrategy.Memory => new MemoryCacheStrategy(_memoryCache, _settings),
            CacheStrategy.Redis => new RedisCacheStrategy(_settings),
            CacheStrategy.File => new FileCacheStrategy(_settings),
            _ => throw new ArgumentException($"Unsupported cache strategy: {strategy}")
        };
    }
}