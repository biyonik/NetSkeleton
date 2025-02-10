namespace Application.Cache.Settings;

/// <summary>
/// Redis ayarları
/// </summary>
public class RedisSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string InstanceName { get; set; } = string.Empty;
}