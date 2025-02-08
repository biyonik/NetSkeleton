namespace Infrastructure.Logging.Settings;

/// <summary>
/// Elastic Search log ayarları
/// </summary>
public class ElasticSearchLogSettings
{
    /// <summary>
    /// Elastic Search URL'i
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Index formatı
    /// </summary>
    public string IndexFormat { get; set; } = "logs-{0:yyyy.MM}";

    /// <summary>
    /// Batch boyutu
    /// </summary>
    public int BatchPostingLimit { get; set; } = 50;

    /// <summary>
    /// Batch gönderim aralığı (saniye)
    /// </summary>
    public int Period { get; set; } = 2;
}