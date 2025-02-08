namespace Infrastructure.Logging.Settings;

/// <summary>
/// Log zenginleştirici ayarları
/// </summary>
public class EnricherSettings
{
    /// <summary>
    /// Uygulama adı
    /// </summary>
    public string ApplicationName { get; set; } = "Application";

    /// <summary>
    /// Ortam adı
    /// </summary>
    public string Environment { get; set; } = "Development";

    /// <summary>
    /// Machine name eklensin mi?
    /// </summary>
    public bool IncludeMachineName { get; set; } = true;

    /// <summary>
    /// Thread ID eklensin mi?
    /// </summary>
    public bool IncludeThreadId { get; set; } = true;
}