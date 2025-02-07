namespace Infrastructure.Cache.Abstractions;

/// <summary>
/// Cache stratejileri için temel interface
/// </summary>
public interface ICacheStrategy
{
    /// <summary>
    /// Cache'den veri getirir
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cache'e veri ekler
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cache'den veri siler
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cache'de key var mı kontrol eder
    /// </summary>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pattern'e uyan tüm key'leri siler
    /// </summary>
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cache'i tamamen temizler
    /// </summary>
    Task ClearAsync(CancellationToken cancellationToken = default);
}