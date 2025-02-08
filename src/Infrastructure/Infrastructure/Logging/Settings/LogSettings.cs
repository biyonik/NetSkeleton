namespace Infrastructure.Logging.Settings;

/// <summary>
/// Logging ayarları
/// </summary>
public class LogSettings
{
    /// <summary>
    /// Minimum log seviyesi
    /// </summary>
    public string MinimumLevel { get; set; } = "Information";

    /// <summary>
    /// Dosya log ayarları
    /// </summary>
    public FileLogSettings? File { get; set; }

    /// <summary>
    /// Console log ayarları
    /// </summary>
    public bool WriteToConsole { get; set; } = true;

    /// <summary>
    /// Elastic Search log ayarları
    /// </summary>
    public ElasticSearchLogSettings? ElasticSearch { get; set; }

    /// <summary>
    /// Log zenginleştirici ayarları
    /// </summary>
    public EnricherSettings Enricher { get; set; } = new();
}
