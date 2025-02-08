using Infrastructure.FileStorage.Abstractions;
using Infrastructure.FileStorage.Settings;
using Infrastructure.FileStorage.Strategies;
using Microsoft.Extensions.Options;

namespace Infrastructure.FileStorage.Factory;

/// <summary>
/// Storage factory implementasyonu
/// </summary>
public class StorageStrategyFactory(IOptions<StorageSettings> settings) : IStorageStrategyFactory
{
    /// <summary>
    /// Verilen strateji tipine göre storage implementasyonu oluşturur
    /// </summary>
    public IStorageStrategy CreateStrategy(StorageStrategy strategy)
    {
        return strategy switch
        {
            StorageStrategy.Local => new LocalStorageStrategy(settings),
            StorageStrategy.AzureBlob => new AzureBlobStorageStrategy(settings),
            StorageStrategy.AmazonS3 => new AmazonS3StorageStrategy(settings),
            _ => throw new ArgumentException($"Unsupported storage strategy: {strategy}")
        };
    }
}