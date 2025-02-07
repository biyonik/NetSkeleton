using Infrastructure.Cache.Abstractions;
using Infrastructure.Cache.Factory;
using Infrastructure.Cache.Settings;
using Microsoft.Extensions.Options;

namespace Infrastructure.Cache;

/// <summary>
/// Cache manager implementasyonu
/// Seçilen stratejiye göre cache işlemlerini yönetir
/// </summary>
public class CacheManager(
    ICacheStrategyFactory cacheStrategyFactory,
    IOptions<CacheSettings> settings)
    : ICacheManager
{
    private readonly ICacheStrategy _cacheStrategy = cacheStrategyFactory.CreateStrategy(settings.Value.Strategy);

    // Settings'den belirlenen stratejiyi kullan

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cacheStrategy.GetAsync<T>(key, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log error
            // Redis bağlantı hatası veya file system hatası durumunda null dön
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cacheStrategy.SetAsync(key, value, expiration, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log error
            // Cache'e yazılamadığında sessizce devam et
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cacheStrategy.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log error
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cacheStrategy.ExistsAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log error
            return false;
        }
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cacheStrategy.RemoveByPatternAsync(pattern, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log error
        }
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _cacheStrategy.ClearAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Log error
        }
    }
}