using Application.Common.Results;
using Domain.Common.Abstractions;

namespace Application.Common.Services;

/// <summary>
/// Tüm servisler için temel interface
/// </summary>
/// <typeparam name="TEntity">Entity tipi</typeparam>
/// <typeparam name="TKey">Entity'nin ID tipi</typeparam>
public interface IBaseService<TEntity, TKey> 
    where TEntity : BaseEntity<TKey>
    where TKey : struct
{
    Task<Result<TEntity>> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<TEntity>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<TEntity>>> GetPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<TKey>> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(TKey id, CancellationToken cancellationToken = default);
}