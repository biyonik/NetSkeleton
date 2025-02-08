using Application.Common.Results;
using Domain.Common.Abstractions;

namespace Application.Common.Services;

/// <summary>
/// DTO bazlı servisler için generic interface
/// </summary>
/// <typeparam name="TDto">DTO tipi</typeparam>
/// <typeparam name="TEntity">Entity tipi</typeparam>
/// <typeparam name="TKey">Entity'nin ID tipi</typeparam>
public interface IBaseDtoService<TDto, TEntity, TKey> 
    where TEntity : BaseEntity<TKey>
    where TKey : struct
{
    Task<Result<TDto>> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<TDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<TDto>>> GetPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<TKey>> CreateAsync(TDto dto, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(TKey id, TDto dto, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(TKey id, CancellationToken cancellationToken = default);
}