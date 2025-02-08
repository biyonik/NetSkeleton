namespace Infrastructure.BackgroundJobs.Settings;

/// <summary>
/// Background job ayarları
/// </summary>
public class BackgroundJobSettings
{
    /// <summary>
    /// Dashboard erişim yolu
    /// </summary>
    public string DashboardPath { get; set; } = "/jobs";

    /// <summary>
    /// Dashboard erişim için kullanıcı adı
    /// </summary>
    public string DashboardUsername { get; set; } = "admin";

    /// <summary>
    /// Dashboard erişim için şifre
    /// </summary>
    public string DashboardPassword { get; set; } = "admin";

    /// <summary>
    /// Worker sayısı
    /// </summary>
    public int WorkerCount { get; set; } = 1;

    /// <summary>
    /// Queue'ları ve ağırlıklarını tanımlar
    /// </summary>
    public Dictionary<string, int> Queues { get; set; } = new()
    {
        { "default", 1 },
        { "critical", 2 },
        { "low", 1 }
    };

    /// <summary>
    /// Storage ayarları
    /// </summary>
    public JobStorageSettings Storage { get; set; } = new();
}