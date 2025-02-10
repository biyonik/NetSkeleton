using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Application.Cache.Abstractions;
using Application.Cache.Settings;
using Microsoft.Extensions.Options;

namespace Application.Cache.Strategies;

/// <summary>
/// File-based cache implementasyonu
/// </summary>
public class FileCacheStrategy : ICacheStrategy
{
    private readonly string _basePath;
    private readonly int _maxFileSizeInMB;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    public FileCacheStrategy(IOptions<CacheSettings> settings)
    {
        var fileSettings = settings.Value.FileCache ?? new FileCacheSettings();
        _basePath = string.IsNullOrWhiteSpace(fileSettings.BasePath) 
            ? Path.Combine(Path.GetTempPath(), "cache")
            : fileSettings.BasePath;
        _maxFileSizeInMB = fileSettings.MaxFileSizeInMB;

        // Cache klasörünü oluştur
        if (!Directory.Exists(_basePath))
            Directory.CreateDirectory(_basePath);
    }

    /// <summary>
    /// Cache'den veri getirir
    /// </summary>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var filePath = GetFilePath(key);
        if (!File.Exists(filePath)) return default;

        try
        {
            await _semaphore.WaitAsync(cancellationToken);

            var cacheEntry = await ReadCacheEntryAsync<T>(filePath, cancellationToken);
            
            if (cacheEntry == null || IsExpired(cacheEntry))
            {
                await RemoveAsync(key, cancellationToken);
                return default;
            }

            return cacheEntry.Value;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Cache'e veri ekler
    /// </summary>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var filePath = GetFilePath(key);
        var cacheEntry = new CacheEntry<T>
        {
            Value = value,
            ExpirationTime = expiration.HasValue 
                ? DateTime.UtcNow.Add(expiration.Value) 
                : null
        };

        try
        {
            await _semaphore.WaitAsync(cancellationToken);

            var json = JsonSerializer.Serialize(cacheEntry);
            var fileInfo = new FileInfo(filePath);

            // Boyut kontrolü
            if (Encoding.UTF8.GetByteCount(json) > _maxFileSizeInMB * 1024 * 1024)
            {
                throw new InvalidOperationException($"Cache entry is too large. Maximum size is {_maxFileSizeInMB}MB");
            }

            await File.WriteAllTextAsync(filePath, json, cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Cache'den veri siler
    /// </summary>
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var filePath = GetFilePath(key);
        if (File.Exists(filePath))
            File.Delete(filePath);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Cache'de key var mı kontrol eder
    /// </summary>
    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var filePath = GetFilePath(key);
        return Task.FromResult(File.Exists(filePath));
    }

    /// <summary>
    /// Pattern'e uyan tüm key'leri siler
    /// </summary>
    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        var regex = new System.Text.RegularExpressions.Regex(pattern, 
            System.Text.RegularExpressions.RegexOptions.Singleline | 
            System.Text.RegularExpressions.RegexOptions.Compiled | 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        var files = Directory.GetFiles(_basePath)
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .Where(f => regex.IsMatch(f));

        foreach (var file in files)
        {
            var filePath = GetFilePath(file);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Cache'i tamamen temizler
    /// </summary>
    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        var files = Directory.GetFiles(_basePath);
        foreach (var file in files)
        {
            File.Delete(file);
        }

        return Task.CompletedTask;
    }

    private string GetFilePath(string key)
    {
        // Key'i hash'le - güvenlik için
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
        var hashString = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

        return Path.Combine(_basePath, $"{hashString}.cache");
    }

    private async Task<CacheEntry<T>?> ReadCacheEntryAsync<T>(string filePath, CancellationToken cancellationToken)
    {
        var json = await File.ReadAllTextAsync(filePath, cancellationToken);
        return JsonSerializer.Deserialize<CacheEntry<T>>(json);
    }

    private bool IsExpired<T>(CacheEntry<T> cacheEntry)
    {
        return cacheEntry.ExpirationTime.HasValue && 
               cacheEntry.ExpirationTime.Value < DateTime.UtcNow;
    }

    private class CacheEntry<T>
    {
        public T? Value { get; set; }
        public DateTime? ExpirationTime { get; set; }
    }
}