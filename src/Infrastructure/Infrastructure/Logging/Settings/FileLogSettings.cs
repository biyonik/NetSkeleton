namespace Infrastructure.Logging.Settings;

/// <summary>
/// Dosya log ayarları
/// </summary>
public class FileLogSettings
{
    /// <summary>
    /// Log dosya yolu
    /// </summary>
    public string Path { get; set; } = "logs/log-.txt";

    /// <summary>
    /// Dosya boyut limiti (bytes)
    /// </summary>
    public long FileSizeLimitBytes { get; set; } = 1073741824; // 1 GB

    /// <summary>
    /// Saklanacak dosya sayısı
    /// </summary>
    public int RetainedFileCountLimit { get; set; } = 31;

    /// <summary>
    /// Log döndürme aralığı
    /// </summary>
    public string RollingInterval { get; set; } = "Day";

    /// <summary>
    /// Log dosyası buffer boyutu
    /// </summary>
    public int BufferSize { get; set; } = 100;
}