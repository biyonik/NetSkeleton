using System.Text.Json;
using Application.Cache.Abstractions;
using Application.Cache.Settings;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Application.Cache.Strategies;

/// <summary>
/// Redis cache implementasyonu
/// </summary>
public class RedisCacheStrategy : ICacheStrategy, IDisposable
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private readonly string _instanceName;
    private bool _disposed;

    public RedisCacheStrategy(IOptions<CacheSettings> settings)
    {
        var redisSettings = settings.Value.Redis ?? throw new ArgumentNullException(nameof(settings), "Redis settings cannot be null");
        
        if (string.IsNullOrEmpty(redisSettings.ConnectionString))
            throw new ArgumentException("Redis connection string cannot be empty", nameof(settings));

        _instanceName = redisSettings.InstanceName;
        _redis = ConnectionMultiplexer.Connect(redisSettings.ConnectionString);
        _db = _redis.GetDatabase();
    }

    /// <summary>
    /// Cache'den veri getirir
    /// </summary>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var redisKey = GetRedisKey(key);
        var value = await _db.StringGetAsync(redisKey);

        if (!value.HasValue)
            return default;

        return JsonSerializer.Deserialize<T>(value!);
    }

    /// <summary>
    /// Cache'e veri ekler
    /// </summary>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var redisKey = GetRedisKey(key);
        var jsonValue = JsonSerializer.Serialize(value);

        if (expiration.HasValue)
            await _db.StringSetAsync(redisKey, jsonValue, expiration);
        else
            await _db.StringSetAsync(redisKey, jsonValue);
    }

    /// <summary>
    /// Cache'den veri siler
    /// </summary>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var redisKey = GetRedisKey(key);
        await _db.KeyDeleteAsync(redisKey);
    }

    /// <summary>
    /// Cache'de key var mı kontrol eder
    /// </summary>
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var redisKey = GetRedisKey(key);
        return await _db.KeyExistsAsync(redisKey);
    }

    /// <summary>
    /// Pattern'e uyan tüm key'leri siler
    /// </summary>
    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        var endPoints = _redis.GetEndPoints();
        var server = _redis.GetServer(endPoints.First());
        
        var redisPattern = GetRedisKey(pattern);
        var keys = server.Keys(pattern: redisPattern);

        foreach (var key in keys)
        {
            await _db.KeyDeleteAsync(key);
        }
    }

    /// <summary>
    /// Cache'i tamamen temizler
    /// </summary>
    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        var endPoints = _redis.GetEndPoints();
        var server = _redis.GetServer(endPoints.First());
        
        // Sadece bu instance'a ait key'leri temizle
        var pattern = GetRedisKey("*");
        var keys = server.Keys(pattern: pattern);

        foreach (var key in keys)
        {
            await _db.KeyDeleteAsync(key);
        }
    }

    /// <summary>
    /// Redis key'ini oluşturur
    /// Instance name ile prefix'ler
    /// </summary>
    private string GetRedisKey(string key)
    {
        return string.IsNullOrEmpty(_instanceName) ? key : $"{_instanceName}:{key}";
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        if (_redis != null)
            _redis.Dispose();

        _disposed = true;
    }

    /// <summary>
    /// Redis bağlantı durumunu kontrol eder
    /// </summary>
    public bool IsConnected => _redis.IsConnected;

    /// <summary>
    /// Redis sunucu bilgilerini getirir
    /// </summary>
    public IEnumerable<string> GetServerInfo()
    {
        var endPoints = _redis.GetEndPoints();

        return (from endPoint in endPoints let server = _redis.GetServer(endPoint) select $"Redis Server: {endPoint}, IsConnected: {server.IsConnected}, Version: {server.Version}").ToList();
    }
}