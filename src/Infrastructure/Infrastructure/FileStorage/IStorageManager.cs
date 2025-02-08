using Infrastructure.FileStorage.Abstractions;

namespace Infrastructure.FileStorage;

/// <summary>
/// Storage işlemlerini yöneten manager interface
/// </summary>
public interface IStorageManager
{
    Task<FileModel> UploadAsync(Stream stream, string fileName, string? folder = null, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);
    Task<Stream> DownloadAsync(string fileId, CancellationToken cancellationToken = default);
    Task DeleteAsync(string fileId, CancellationToken cancellationToken = default);
    Task<FileModel?> GetInfoAsync(string fileId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string fileId, CancellationToken cancellationToken = default);
    Task<string> GetUrlAsync(string fileId, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
}