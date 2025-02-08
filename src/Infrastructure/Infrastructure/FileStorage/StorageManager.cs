using Infrastructure.FileStorage.Abstractions;
using Infrastructure.FileStorage.Exceptions;
using Infrastructure.FileStorage.Factory;
using Infrastructure.FileStorage.Settings;
using Microsoft.Extensions.Options;

namespace Infrastructure.FileStorage;

/// <summary>
/// Storage manager implementasyonu
/// Seçilen stratejiye göre storage işlemlerini yönetir
/// </summary>
public class StorageManager(
    IStorageStrategyFactory strategyFactory,
    IOptions<StorageSettings> settings)
    : IStorageManager, IDisposable
{
    private readonly IStorageStrategy _storageStrategy = strategyFactory.CreateStrategy(settings.Value.Strategy);
    private bool _disposed;

    /// <summary>
    /// Dosya yükler
    /// </summary>
    public async Task<FileModel> UploadAsync(
        Stream stream,
        string fileName,
        string? folder = null,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateUpload(stream, fileName);
            return await _storageStrategy.UploadAsync(stream, fileName, folder, metadata, cancellationToken);
        }
        catch (Exception ex) when (ex is not StorageException)
        {
            throw new StorageException($"Failed to upload file: {fileName}", ex);
        }
    }

    /// <summary>
    /// Dosya indirir
    /// </summary>
    public async Task<Stream> DownloadAsync(string fileId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _storageStrategy.DownloadAsync(fileId, cancellationToken);
        }
        catch (Exception ex) when (ex is not StorageException)
        {
            throw new StorageException($"Failed to download file: {fileId}", ex);
        }
    }

    /// <summary>
    /// Dosyayı siler
    /// </summary>
    public async Task DeleteAsync(string fileId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _storageStrategy.DeleteAsync(fileId, cancellationToken);
        }
        catch (Exception ex) when (ex is not StorageException)
        {
            throw new StorageException($"Failed to delete file: {fileId}", ex);
        }
    }

    /// <summary>
    /// Dosya bilgilerini getirir
    /// </summary>
    public async Task<FileModel?> GetInfoAsync(string fileId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _storageStrategy.GetInfoAsync(fileId, cancellationToken);
        }
        catch (Exception ex) when (ex is not StorageException)
        {
            throw new StorageException($"Failed to get file info: {fileId}", ex);
        }
    }

    /// <summary>
    /// Dosya var mı kontrol eder
    /// </summary>
    public async Task<bool> ExistsAsync(string fileId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _storageStrategy.ExistsAsync(fileId, cancellationToken);
        }
        catch (Exception ex) when (ex is not StorageException)
        {
            throw new StorageException($"Failed to check file existence: {fileId}", ex);
        }
    }

    /// <summary>
    /// Dosya URL'i oluşturur
    /// </summary>
    public async Task<string> GetUrlAsync(string fileId, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _storageStrategy.GetUrlAsync(fileId, expiry, cancellationToken);
        }
        catch (Exception ex) when (ex is not StorageException)
        {
            throw new StorageException($"Failed to generate URL for file: {fileId}", ex);
        }
    }

    /// <summary>
    /// Upload validasyonu yapar
    /// </summary>
    private void ValidateUpload(Stream stream, string fileName)
    {
        if (stream == null || stream.Length == 0)
            throw new StorageValidationException("File stream is empty");

        if (string.IsNullOrWhiteSpace(fileName))
            throw new StorageValidationException("File name is required");

        if (Path.GetInvalidFileNameChars().Any(c => fileName.Contains(c)))
            throw new StorageValidationException("File name contains invalid characters");
    }

    public void Dispose()
    {
        if (_disposed) return;

        if (_storageStrategy is IDisposable disposable)
        {
            disposable.Dispose();
        }

        _disposed = true;
    }
}