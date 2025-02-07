using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Infrastructure.Cache.Abstractions;
using Infrastructure.Cache.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Infrastructure.Cache.Strategies;

/// <summary>
/// In-Memory cache implementasyonu
/// </summary>
public class MemoryCacheStrategy : ICacheStrategy
{
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheSettings _settings;
    private readonly ConcurrentDictionary<string, object> _keys = new();

    public MemoryCacheStrategy(
        IMemoryCache cache,
        IOptions<CacheSettings> settings)
    {
        _cache = cache;
        _settings = settings.Value.MemoryCache ?? new MemoryCacheSettings();
    }

    /// <summary>
    /// Cache'den veri getirir
    /// </summary>
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = _cache.Get<T>(key);
        return Task.FromResult(value);
    }

    /// <summary>
    /// Cache'e veri ekler
    /// </summary>
    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var options = new MemoryCacheEntryOptions
        {
            SlidingExpiration = expiration ?? TimeSpan.FromMinutes(_settings.SlidingExpirationInMinutes),
            Size = 1 // Her entry için 1 birim boyut
        };

        _cache.Set(key, value, options);
        _keys.TryAdd(key, true);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Cache'den veri siler
    /// </summary>
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.Remove(key);
        _keys.TryRemove(key, out _);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Cache'de key var mı kontrol eder
    /// </summary>
    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_keys.ContainsKey(key));
    }

    /// <summary>
    /// Pattern'e uyan tüm key'leri siler
    /// </summary>
    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        var keysToRemove = _keys.Keys
            .Where(k => regex.IsMatch(k))
            .ToList();

        foreach (var key in keysToRemove)
        {
            _cache.Remove(key);
            _keys.TryRemove(key, out _);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Cache'i tamamen temizler
    /// </summary>
    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        foreach (var key in _keys.Keys)
        {
            _cache.Remove(key);
        }
        
        _keys.Clear();
        
        return Task.CompletedTask;
    }
}