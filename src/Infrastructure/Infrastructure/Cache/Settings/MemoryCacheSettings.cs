namespace Infrastructure.Cache.Settings;

/// <summary>
/// Memory cache ayarları
/// </summary>
public class MemoryCacheSettings
{
    public long SizeLimit { get; set; } = 1024;
    public int SlidingExpirationInMinutes { get; set; } = 60;
}