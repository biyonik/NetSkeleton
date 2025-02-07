using Domain.Common.Abstractions;
using Domain.Common.Specifications;

namespace Domain.Common.Interfaces;

/// <summary>
/// Generic repository interface
/// </summary>
/// <typeparam name="T">Entity tipi</typeparam>
/// <typeparam name="TKey">Entity'nin primary key tipi</typeparam>
public interface IRepository<T, in TKey> where T : BaseEntity<TKey> where TKey : struct
{
    /// <summary>
    /// Entity'yi ID ile getirir
    /// </summary>
    Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Specification'a göre tek bir entity getirir
    /// </summary>
    Task<T?> SingleOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Tüm entity'leri getirir
    /// </summary>
    Task<List<T>> ListAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Specification'a uyan tüm entity'leri getirir
    /// </summary>
    Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Yeni entity ekler
    /// </summary>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Entity'yi günceller
    /// </summary>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Entity'yi siler
    /// </summary>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Specification'a uyan kayıt sayısını getirir
    /// </summary>
    Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Specification'a uyan kayıt var mı kontrol eder
    /// </summary>
    Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
}