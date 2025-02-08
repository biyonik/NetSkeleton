namespace Infrastructure.BackgroundJobs.Settings;

/// <summary>
/// Job storage ayarları
/// </summary>
public class JobStorageSettings
{
    /// <summary>
    /// PostgreSQL bağlantı string'i
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Schema adı
    /// </summary>
    public string SchemaName { get; set; } = "hangfire";

    /// <summary>
    /// Tablo prefix'i
    /// </summary>
    public string TablePrefix { get; set; } = "hangfire";
}