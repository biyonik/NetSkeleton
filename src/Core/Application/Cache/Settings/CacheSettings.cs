using Application.Cache.Strategies;

namespace Application.Cache.Settings;

/// <summary>
/// Cache servisinin kullanacağı ayarlar
/// </summary>
public class CacheSettings
{
    /// <summary>
    /// Kullanılacak cache stratejisi
    /// </summary>
    public CacheStrategy Strategy { get; set; } = CacheStrategy.Memory;

    /// <summary>
    /// Redis ayarları
    /// </summary>
    public RedisSettings? Redis { get; set; }

    /// <summary>
    /// File cache ayarları
    /// </summary>
    public FileCacheSettings? FileCache { get; set; }

    /// <summary>
    /// Memory cache ayarları
    /// </summary>
    public MemoryCacheSettings? MemoryCache { get; set; }
}