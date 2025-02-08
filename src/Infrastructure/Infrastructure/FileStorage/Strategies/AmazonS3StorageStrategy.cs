using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Infrastructure.FileStorage.Abstractions;
using Infrastructure.FileStorage.Exceptions;
using Infrastructure.FileStorage.Settings;
using Microsoft.Extensions.Options;
using MimeTypes;

namespace Infrastructure.FileStorage.Strategies;

/// <summary>
/// Amazon S3 storage stratejisi
/// </summary>
public class AmazonS3StorageStrategy : IStorageStrategy, IDisposable
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private bool _disposed;

    public AmazonS3StorageStrategy(IOptions<StorageSettings> settings)
    {
        var s3Settings = settings.Value.AmazonS3 ?? throw new ArgumentNullException(nameof(settings));

        // S3 client oluştur
        _s3Client = new AmazonS3Client(
            s3Settings.AccessKey,
            s3Settings.SecretKey,
            Amazon.RegionEndpoint.GetBySystemName(s3Settings.Region));

        _bucketName = s3Settings.BucketName;

        // Bucket'ı kontrol et
        EnsureBucketExists().GetAwaiter().GetResult();
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

        // S3 key oluştur
        var s3Key = BuildS3Key(fileModel.Id, folder);

        // Meta verileri hazırla
        var objectMetadata = new Dictionary<string, string>(fileModel.Metadata)
        {
            { "fileName", fileModel.FileName },
            { "mimeType", fileModel.MimeType },
            { "fileId", fileModel.Id }
        };
        
        // Upload request oluştur
        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = stream,
            BucketName = _bucketName,
            Key = s3Key,
            ContentType = fileModel.MimeType,
        };
        
        foreach (var (key, value) in objectMetadata)
        {
            uploadRequest.Metadata[key] = value;
        }

        // Dosyayı yükle
        var transferUtility = new TransferUtility(_s3Client);
        await transferUtility.UploadAsync(uploadRequest, cancellationToken);

        fileModel.Path = s3Key;
        return fileModel;
    }

    /// <summary>
    /// Dosya indirir
    /// </summary>
    public async Task<Stream> DownloadAsync(string fileId, CancellationToken cancellationToken = default)
    {
        var s3Key = BuildS3Key(fileId, null);

        try
        {
            var response = await _s3Client.GetObjectAsync(_bucketName, s3Key, cancellationToken);
            
            var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream, cancellationToken);
            memoryStream.Position = 0;
            
            return memoryStream;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new StorageException($"File not found: {fileId}");
        }
    }

    /// <summary>
    /// Dosyayı siler
    /// </summary>
    public async Task DeleteAsync(string fileId, CancellationToken cancellationToken = default)
    {
        var s3Key = BuildS3Key(fileId, null);

        try
        {
            await _s3Client.DeleteObjectAsync(_bucketName, s3Key, cancellationToken);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode != System.Net.HttpStatusCode.NotFound)
        {
            throw new StorageException($"Failed to delete file: {fileId}", ex);
        }
    }

    /// <summary>
    /// Dosya bilgilerini getirir
    /// </summary>
    public async Task<FileModel?> GetInfoAsync(string fileId, CancellationToken cancellationToken = default)
    {
        var s3Key = BuildS3Key(fileId, null);

        try
        {
            var response = await _s3Client.GetObjectMetadataAsync(_bucketName, s3Key, cancellationToken);

            // MetadataCollection'ı Dictionary'ye çevirme
            var metadata = new Dictionary<string, string>();
            foreach (var key in response.Metadata.Keys)
            {
                metadata[key] = response.Metadata[key];
            }

            var fileModel = new FileModel
            {
                Id = fileId,
                FileName = metadata["fileName"],
                MimeType = response.Headers.ContentType,
                Size = response.ContentLength,
                Path = s3Key,
                CreatedAt = response.LastModified,
                Metadata = metadata
            };
        
            return fileModel;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }


    /// <summary>
    /// Dosya var mı kontrol eder
    /// </summary>
    public async Task<bool> ExistsAsync(string fileId, CancellationToken cancellationToken = default)
    {
        var s3Key = BuildS3Key(fileId, null);

        try
        {
            await _s3Client.GetObjectMetadataAsync(_bucketName, s3Key, cancellationToken);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    /// <summary>
    /// Pre-signed URL oluşturur
    /// </summary>
    public async Task<string> GetUrlAsync(string fileId, TimeSpan? expiry = default, CancellationToken cancellationToken = default)
    {
        var s3Key = BuildS3Key(fileId, null);

        if (!await ExistsAsync(fileId, cancellationToken))
            throw new StorageException($"File not found: {fileId}");

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = s3Key,
            Expires = DateTime.UtcNow.Add(expiry ?? TimeSpan.FromHours(1))
        };

        return _s3Client.GetPreSignedURL(request);
    }

    private string BuildS3Key(string fileId, string? folder)
    {
        return string.IsNullOrEmpty(folder)
            ? fileId
            : $"{folder}/{fileId}";
    }

    private async Task EnsureBucketExists()
    {
        try
        {
            var response = await _s3Client.ListBucketsAsync();
            if (!response.Buckets.Any(b => b.BucketName == _bucketName))
            {
                await _s3Client.PutBucketAsync(_bucketName);
            }
        }
        catch (AmazonS3Exception ex)
        {
            throw new StorageException("Failed to initialize S3 bucket", ex);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        _s3Client?.Dispose();
        _disposed = true;
    }
}