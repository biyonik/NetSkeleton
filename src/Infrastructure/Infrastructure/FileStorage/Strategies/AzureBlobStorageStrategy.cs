using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Infrastructure.FileStorage.Abstractions;
using Infrastructure.FileStorage.Exceptions;
using Infrastructure.FileStorage.Settings;
using Microsoft.Extensions.Options;
using MimeTypes;

namespace Infrastructure.FileStorage.Strategies;

/// <summary>
/// Azure Blob Storage stratejisi
/// </summary>
public class AzureBlobStorageStrategy : IStorageStrategy
{
    private readonly BlobContainerClient _containerClient;

    public AzureBlobStorageStrategy(IOptions<StorageSettings> settings)
    {
        var azureSettings = settings.Value.AzureBlob ?? throw new ArgumentNullException(nameof(settings));

        // Container client'ı oluştur
        var blobServiceClient = new BlobServiceClient(azureSettings.ConnectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(azureSettings.ContainerName);
        
        // Container'ı oluştur (yoksa)
        _containerClient.CreateIfNotExists();
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
        var fileModel = new FileModel
        {
            FileName = fileName,
            Extension = Path.GetExtension(fileName).ToLowerInvariant(),
            MimeType = MimeTypeMap.GetMimeType(Path.GetExtension(fileName)),
            Size = stream.Length,
            Metadata = metadata ?? new Dictionary<string, string>()
        };

        // Blob path'ini oluştur
        var blobPath = BuildBlobPath(fileModel.Id, folder);
        var blobClient = _containerClient.GetBlobClient(blobPath);

        // Meta verileri hazırla
        var blobMetadata = new Dictionary<string, string>
        {
            { "fileName", fileModel.FileName },
            { "mimeType", fileModel.MimeType },
            { "fileId", fileModel.Id }
        };

        // Kullanıcının meta verilerini ekle
        foreach (var meta in fileModel.Metadata)
        {
            blobMetadata[meta.Key] = meta.Value;
        }

        // Upload options
        var options = new BlobUploadOptions
        {
            Metadata = blobMetadata,
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = fileModel.MimeType
            }
        };

        // Dosyayı yükle
        stream.Position = 0;
        await blobClient.UploadAsync(stream, options, cancellationToken);

        fileModel.Path = blobPath;
        return fileModel;
    }

    /// <summary>
    /// Dosya indirir
    /// </summary>
    public async Task<Stream> DownloadAsync(string fileId, CancellationToken cancellationToken = default)
    {
        var blobPath = BuildBlobPath(fileId, null);
        var blobClient = _containerClient.GetBlobClient(blobPath);

        if (!await blobClient.ExistsAsync(cancellationToken))
            throw new StorageException($"File not found: {fileId}");

        var memoryStream = new MemoryStream();
        await blobClient.DownloadToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;
        
        return memoryStream;
    }

    /// <summary>
    /// Dosyayı siler
    /// </summary>
    public async Task DeleteAsync(string fileId, CancellationToken cancellationToken = default)
    {
        var blobPath = BuildBlobPath(fileId, null);
        var blobClient = _containerClient.GetBlobClient(blobPath);
        
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Dosya bilgilerini getirir
    /// </summary>
    public async Task<FileModel?> GetInfoAsync(string fileId, CancellationToken cancellationToken = default)
    {
        var blobPath = BuildBlobPath(fileId, null);
        var blobClient = _containerClient.GetBlobClient(blobPath);

        if (!await blobClient.ExistsAsync(cancellationToken))
            return null;

        var properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);

        return new FileModel
        {
            Id = fileId,
            FileName = properties.Value.Metadata["fileName"],
            MimeType = properties.Value.ContentType,
            Size = properties.Value.ContentLength,
            Path = blobPath,
            Metadata = properties.Value.Metadata.ToDictionary(x => x.Key, x => x.Value),
            CreatedAt = properties.Value.CreatedOn.UtcDateTime
        };
    }

    /// <summary>
    /// Dosya var mı kontrol eder
    /// </summary>
    public async Task<bool> ExistsAsync(string fileId, CancellationToken cancellationToken = default)
    {
        var blobPath = BuildBlobPath(fileId, null);
        var blobClient = _containerClient.GetBlobClient(blobPath);
        
        return await blobClient.ExistsAsync(cancellationToken);
    }

    /// <summary>
    /// Dosya URL'i oluşturur (Shared Access Signature ile)
    /// </summary>
    public async Task<string> GetUrlAsync(string fileId, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var blobPath = BuildBlobPath(fileId, null);
        var blobClient = _containerClient.GetBlobClient(blobPath);

        if (!await blobClient.ExistsAsync(cancellationToken))
            throw new StorageException($"File not found: {fileId}");

        // Default olarak 1 saatlik URL
        var sasExpiry = expiry ?? TimeSpan.FromHours(1);
        
        var sasUri = blobClient.GenerateSasUri(Azure.Storage.Sas.BlobSasPermissions.Read, 
            DateTimeOffset.UtcNow.Add(sasExpiry));

        return sasUri.ToString();
    }

    private string BuildBlobPath(string fileId, string? folder)
    {
        return string.IsNullOrEmpty(folder)
            ? fileId
            : $"{folder}/{fileId}";
    }
}