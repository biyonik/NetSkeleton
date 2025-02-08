using Domain.Common.Abstractions;
using Domain.Common.Interfaces;
using Domain.Common.Specifications;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Extensions;

namespace Persistence.Repositories;

/// <summary>
/// Generic repository implementasyonu
/// </summary>
/// <typeparam name="T">Entity tipi</typeparam>
/// <typeparam name="TKey">Entity'nin primary key tipi</typeparam>
public class BaseRepository<T, TKey>(ApplicationDbContext context) : IRepository<T, TKey>, IReadRepository<T, TKey>
    where T : BaseEntity<TKey>
    where TKey : struct
{
    protected readonly ApplicationDbContext Context = context;
    protected readonly DbSet<T> DbSet = context.Set<T>();

    /// <summary>
    /// Entity'yi ID ile getirir
    /// </summary>
    public virtual async Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    /// <summary>
    /// Specification'a göre tek bir entity getirir
    /// </summary>
    public virtual async Task<T?> SingleOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Tüm entity'leri getirir
    /// </summary>
    public virtual async Task<List<T>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Specification'a uyan tüm entity'leri getirir
    /// </summary>
    public virtual async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Yeni entity ekler
    /// </summary>
    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    /// <summary>
    /// Entity'yi günceller
    /// </summary>
    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        Context.Entry(entity).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    /// <summary>
    /// Entity'yi siler
    /// </summary>
    public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Specification'a uyan kayıt sayısını getirir
    /// </summary>
    public virtual async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        return await SpecificationEvaluator<T>.CountAsync(DbSet.AsQueryable(), specification);
    }

    /// <summary>
    /// Specification'a uyan kayıt var mı kontrol eder
    /// </summary>
    public virtual async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).AnyAsync(cancellationToken);
    }

    /// <summary>
    /// AsNoTracking ile sorgu yapar
    /// </summary>
    protected virtual IQueryable<T> AsNoTracking()
    {
        return DbSet.AsNoTracking();
    }

    /// <summary>
    /// Specification'ı query'ye uygular
    /// </summary>
    protected virtual IQueryable<T> ApplySpecification(ISpecification<T> specification)
    {
        return SpecificationEvaluator<T>.GetQuery(DbSet.AsQueryable(), specification);
    }

    /// <summary>
    /// AsNoTracking ile specification'ı query'ye uygular
    /// </summary>
    protected virtual IQueryable<T> ApplySpecificationAsNoTracking(ISpecification<T> specification)
    {
        return SpecificationEvaluator<T>.GetQuery(DbSet.AsNoTracking(), specification);
    }
}