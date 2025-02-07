using Domain.Common.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Extensions;

/// <summary>
/// Specification pattern'i EF Core query'lerine çeviren yardımcı sınıf
/// </summary>
public static class SpecificationEvaluator<T> where T : class
{
    /// <summary>
    /// Specification'ı EF Core query'sine çevirir
    /// </summary>
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
    {
        var query = inputQuery;

        // Kriterleri uygula
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        // Include'ları uygula
        query = specification.Includes.Aggregate(query,
            (current, include) => current.Include(include));

        // String Include'ları uygula
        query = specification.IncludeStrings.Aggregate(query,
            (current, include) => current.Include(include));

        // Sıralama - OrderBy
        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        
        // Sıralama - OrderByDescending
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        // Gruplama
        if (specification.GroupBy != null)
        {
            query = query.GroupBy(specification.GroupBy).SelectMany(x => x);
        }

        // Sayfalama
        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip)
                .Take(specification.Take);
        }

        return query;
    }

    /// <summary>
    /// Specification'a göre kayıt sayısını hesaplar
    /// </summary>
    public static async Task<int> CountAsync(IQueryable<T> inputQuery, ISpecification<T> specification)
    {
        var query = inputQuery;

        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        return await query.CountAsync();
    }
}