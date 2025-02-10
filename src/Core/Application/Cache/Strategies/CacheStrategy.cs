namespace Application.Cache.Strategies;

/// <summary>
/// Kullanılabilir cache stratejileri
/// </summary>
public enum CacheStrategy
{
    Memory = 1,
    Redis = 2,
    File = 3
}