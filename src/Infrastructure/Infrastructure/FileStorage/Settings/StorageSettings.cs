namespace Infrastructure.FileStorage.Settings;

/// <summary>
/// Storage ayarları
/// </summary>
public class StorageSettings
{
    /// <summary>
    /// Kullanılacak storage stratejisi
    /// </summary>
    public StorageStrategy Strategy { get; set; } = StorageStrategy.Local;

    /// <summary>
    /// Lokal dosya sistemi ayarları
    /// </summary>
    public LocalStorageSettings? Local { get; set; }

    /// <summary>
    /// Azure Blob Storage ayarları
    /// </summary>
    public AzureBlobStorageSettings? AzureBlob { get; set; }

    /// <summary>
    /// Amazon S3 ayarları
    /// </summary>
    public AmazonS3Settings? AmazonS3 { get; set; }
}