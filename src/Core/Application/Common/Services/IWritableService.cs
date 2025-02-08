using Application.Common.Results;
using Domain.Common.Abstractions;

namespace Application.Common.Services;

/// <summary>
/// CRUD servisler için interface
/// </summary>
/// <typeparam name="TEntity">Entity tipi</typeparam>
/// <typeparam name="TKey">Entity'nin ID tipi</typeparam>
public interface IWritableService<TEntity, TKey> : IReadableService<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : struct
{
    Task<Result<TKey>> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
}