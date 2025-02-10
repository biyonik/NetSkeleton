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
public sealed class BaseRepository<T, TKey>(ApplicationDbContext context) : IRepository<T, TKey>, IReadRepository<T, TKey>
    where T : BaseEntity<TKey>
    where TKey : struct
{
    private readonly DbSet<T> _dbSet = context.Set<T>();

    /// <summary>
    /// Entity'yi ID ile getirir
    /// </summary>
    public async Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    /// <summary>
    /// Specification'a göre tek bir entity getirir
    /// </summary>
    public async Task<T?> SingleOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Tüm entity'leri getirir
    /// </summary>
    public async Task<List<T>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Specification'a uyan tüm entity'leri getirir
    /// </summary>
    public async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Yeni entity ekler
    /// </summary>
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    /// <summary>
    /// Entity'yi günceller
    /// </summary>
    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        context.Entry(entity).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    /// <summary>
    /// Entity'yi siler
    /// </summary>
    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Specification'a uyan kayıt sayısını getirir
    /// </summary>
    public async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        return await SpecificationEvaluator<T>.CountAsync(_dbSet.AsQueryable(), specification);
    }

    /// <summary>
    /// Specification'a uyan kayıt var mı kontrol eder
    /// </summary>
    public async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).AnyAsync(cancellationToken);
    }

    /// <summary>
    /// AsNoTracking ile sorgu yapar
    /// </summary>
    private IQueryable<T> AsNoTracking()
    {
        return _dbSet.AsNoTracking();
    }

    /// <summary>
    /// Specification'ı query'ye uygular
    /// </summary>
    private IQueryable<T> ApplySpecification(ISpecification<T> specification)
    {
        return SpecificationEvaluator<T>.GetQuery(_dbSet.AsQueryable(), specification);
    }

    /// <summary>
    /// AsNoTracking ile specification'ı query'ye uygular
    /// </summary>
    private IQueryable<T> ApplySpecificationAsNoTracking(ISpecification<T> specification)
    {
        return SpecificationEvaluator<T>.GetQuery(_dbSet.AsNoTracking(), specification);
    }
}