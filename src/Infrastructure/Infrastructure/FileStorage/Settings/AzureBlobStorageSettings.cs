namespace Infrastructure.FileStorage.Settings;

/// <summary>
/// Azure Blob Storage ayarları
/// </summary>
public class AzureBlobStorageSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
}