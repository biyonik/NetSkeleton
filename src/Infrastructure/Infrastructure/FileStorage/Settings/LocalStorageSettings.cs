namespace Infrastructure.FileStorage.Settings;

/// <summary>
/// Lokal dosya sistemi ayarları
/// </summary>
public class LocalStorageSettings
{
    public string BasePath { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public long MaxFileSizeInMB { get; set; } = 100;
}