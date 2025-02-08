namespace Infrastructure.FileStorage.Abstractions;

/// <summary>
/// Storage stratejileri için temel interface
/// </summary>
public interface IStorageStrategy
{
    /// <summary>
    /// Dosya yükler
    /// </summary>
    Task<FileModel> UploadAsync(Stream stream, string fileName, string? folder = null, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dosya indirir
    /// </summary>
    Task<Stream> DownloadAsync(string fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dosyayı siler
    /// </summary>
    Task DeleteAsync(string fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dosya bilgilerini getirir
    /// </summary>
    Task<FileModel?> GetInfoAsync(string fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dosya var mı kontrol eder
    /// </summary>
    Task<bool> ExistsAsync(string fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dosya URL'i oluşturur
    /// </summary>
    Task<string> GetUrlAsync(string fileId, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
}