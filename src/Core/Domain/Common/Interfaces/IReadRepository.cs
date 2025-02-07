using Domain.Common.Abstractions;
using Domain.Common.Specifications;

namespace Domain.Common.Interfaces;


/// <summary>
/// Read-only repository interface
/// Command-Query Separation (CQS) için kullanılır
/// </summary>
public interface IReadRepository<T, in TKey> where T : BaseEntity<TKey> where TKey : struct
{
    Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<T?> SingleOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    Task<List<T>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
}