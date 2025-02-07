using Infrastructure.Cache.Abstractions;
using Infrastructure.Cache.Strategies;

namespace Infrastructure.Cache.Factory;

/// <summary>
/// Cache stratejisi oluşturmak için factory
/// </summary>
public interface ICacheStrategyFactory
{
    /// <summary>
    /// İstenilen cache stratejisini oluşturur
    /// </summary>
    ICacheStrategy CreateStrategy(CacheStrategy strategy);
}