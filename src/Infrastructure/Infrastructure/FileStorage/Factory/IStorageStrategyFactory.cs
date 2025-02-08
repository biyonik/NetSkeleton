using Infrastructure.FileStorage.Abstractions;
using Infrastructure.FileStorage.Settings;

namespace Infrastructure.FileStorage.Factory;

/// <summary>
/// Storage stratejisi oluşturmak için factory interface
/// </summary>
public interface IStorageStrategyFactory
{
    /// <summary>
    /// İstenilen storage stratejisini oluşturur
    /// </summary>
    IStorageStrategy CreateStrategy(StorageStrategy strategy);
}