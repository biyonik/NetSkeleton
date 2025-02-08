using System.Text.Json;
using Infrastructure.FileStorage.Abstractions;
using Infrastructure.FileStorage.Exceptions;
using Infrastructure.FileStorage.Settings;
using Microsoft.Extensions.Options;
using MimeTypes;

namespace Infrastructure.FileStorage.Strategies;

/// <summary>
/// Lokal dosya sistemi storage stratejisi
/// </summary>
public class LocalStorageStrategy : IStorageStrategy
{
    private readonly string _basePath;
    private readonly string _baseUrl;
    private readonly long _maxFileSizeInBytes;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    public LocalStorageStrategy(IOptions<StorageSettings> settings)
    {
        var localSettings = settings.Value.Local ?? throw new ArgumentNullException(nameof(settings));
        
        _basePath = string.IsNullOrWhiteSpace(localSettings.BasePath)
            ? Path.Combine(Path.GetTempPath(), "storage")
            : localSettings.BasePath;

        _baseUrl = localSettings.BaseUrl.TrimEnd('/');
        _maxFileSizeInBytes = localSettings.MaxFileSizeInMB * 1024 * 1024;

        // Ana dizini ve alt dizinleri oluştur
        EnsureDirectoriesExist();
    }

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
        if (stream.Length > _maxFileSizeInBytes)
            throw new StorageException($"File size exceeds maximum allowed size of {_maxFileSizeInBytes / 1024 / 1024}MB");

        var fileModel = new FileModel
        {
            FileName = fileName,
            Extension = Path.GetExtension(fileName).ToLowerInvariant(),
            MimeType = MimeTypeMap.GetMimeType(Path.GetExtension(fileName)),
            Size = stream.Length,
            Metadata = metadata ?? new Dictionary<string, string>()
        };

        var relativePath = BuildRelativePath(fileModel.Id, folder);
        var fullPath = Path.Combine(_basePath, relativePath);
        var metaPath = Path.ChangeExtension(fullPath, ".meta.json");

        try
        {
            await _semaphore.WaitAsync(cancellationToken);

            // Dosya dizinini oluştur
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            // Dosyayı kaydet
            using (var fileStream = File.Create(fullPath))
            {
                stream.Position = 0;
                await stream.CopyToAsync(fileStream, cancellationToken);
            }

            // Meta verileri kaydet
            await File.WriteAllTextAsync(
                metaPath,
                JsonSerializer.Serialize(fileModel),
                cancellationToken);

            fileModel.Path = relativePath;
            return fileModel;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Dosya indirir
    /// </summary>
    public async Task<Stream> DownloadAsync(string fileId, CancellationToken cancellationToken = default)
    {
        var filePath = await GetFilePathAsync(fileId, cancellationToken);
        if (!File.Exists(filePath))
            throw new StorageException($"File not found: {fileId}");

        return File.OpenRead(filePath);
    }

    /// <summary>
    /// Dosyayı siler
    /// </summary>
    public async Task DeleteAsync(string fileId, CancellationToken cancellationToken = default)
    {
        try
        {
            var filePath = await GetFilePathAsync(fileId, cancellationToken);
            var metaPath = Path.ChangeExtension(filePath, ".meta.json");

            await _semaphore.WaitAsync(cancellationToken);

            if (File.Exists(filePath))
                File.Delete(filePath);

            if (File.Exists(metaPath))
                File.Delete(metaPath);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Dosya bilgilerini getirir
    /// </summary>
    public async Task<FileModel?> GetInfoAsync(string fileId, CancellationToken cancellationToken = default)
    {
        var filePath = await GetFilePathAsync(fileId, cancellationToken);
        var metaPath = Path.ChangeExtension(filePath, ".meta.json");

        if (!File.Exists(metaPath))
            return null;

        var json = await File.ReadAllTextAsync(metaPath, cancellationToken);
        return JsonSerializer.Deserialize<FileModel>(json);
    }

    /// <summary>
    /// Dosya var mı kontrol eder
    /// </summary>
    public async Task<bool> ExistsAsync(string fileId, CancellationToken cancellationToken = default)
    {
        var filePath = await GetFilePathAsync(fileId, cancellationToken);
        return File.Exists(filePath);
    }

    /// <summary>
    /// Dosya URL'i oluşturur
    /// </summary>
    public Task<string> GetUrlAsync(string fileId, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_baseUrl))
            throw new StorageException("Base URL is not configured");

        var relativePath = BuildRelativePath(fileId, null);
        var url = $"{_baseUrl}/{relativePath}";

        return Task.FromResult(url);
    }

    private void EnsureDirectoriesExist()
    {
        if (!Directory.Exists(_basePath))
            Directory.CreateDirectory(_basePath);

        // Alt dizinler için hash tabanlı klasör yapısı
        for (var i = 0; i < 16; i++)
        {
            var subDir = Path.Combine(_basePath, i.ToString("X"));
            if (!Directory.Exists(subDir))
                Directory.CreateDirectory(subDir);
        }
    }

    private string BuildRelativePath(string fileId, string? folder)
    {
        // Hash tabanlı dizin yapısı
        var subDir = fileId[0].ToString();
        
        return string.IsNullOrEmpty(folder)
            ? Path.Combine(subDir, fileId)
            : Path.Combine(folder, subDir, fileId);
    }

    private Task<string> GetFilePathAsync(string fileId, CancellationToken cancellationToken)
    {
        var relativePath = BuildRelativePath(fileId, null);
        return Task.FromResult(Path.Combine(_basePath, relativePath));
    }
}