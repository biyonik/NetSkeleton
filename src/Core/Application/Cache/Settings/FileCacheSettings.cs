namespace Application.Cache.Settings;

/// <summary>
/// File cache ayarları
/// </summary>
public class FileCacheSettings
{
    public string BasePath { get; set; } = string.Empty;
    public int MaxFileSizeInMB { get; set; } = 100;
}