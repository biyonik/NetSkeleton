using Application.Common.Results;
using Domain.Common.Abstractions;

namespace Application.Common.Services;

/// <summary>
/// Read-only servisler için interface
/// </summary>
/// <typeparam name="TEntity">Entity tipi</typeparam>
/// <typeparam name="TKey">Entity'nin ID tipi</typeparam>
public interface IReadableService<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : struct
{
    Task<Result<TEntity>> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<TEntity>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<TEntity>>> GetPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(TKey id, CancellationToken cancellationToken = default);
}