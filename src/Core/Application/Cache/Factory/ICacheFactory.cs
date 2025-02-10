using Application.Cache.Abstractions;
using Application.Cache.Strategies;

namespace Application.Cache.Factory;

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