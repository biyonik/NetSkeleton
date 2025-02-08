namespace Application.Common.CQRS;

/// <summary>
/// Cache'lenebilir query'ler için interface
/// </summary>
public interface ICacheableQuery<TResponse> : IQuery<TResponse>
{
    /// <summary>
    /// Cache key'i
    /// </summary>
    string CacheKey { get; }

    /// <summary>
    /// Cache süresi
    /// </summary>
    TimeSpan? CacheExpiration { get; }
}