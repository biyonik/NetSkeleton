using Microsoft.EntityFrameworkCore;

namespace Domain.Common.Specifications;


public static class SpecificationExtensions
{
    public static IQueryable<T> WithSpecification<T>(
        this IQueryable<T> query, 
        ISpecification<T> specification) where T : class
    {
        // Criteria uygula
        query = query.Where(specification.Criteria);

        // Include'ları uygula
        query = specification.Includes.Aggregate(query,
            (current, include) => current.Include(include));

        // String Include'ları uygula
        query = specification.IncludeStrings.Aggregate(query,
            (current, include) => current.Include(include));

        // Sıralama
        if (specification.OrderBy != null)
            query = query.OrderBy(specification.OrderBy);
        else if (specification.OrderByDescending != null)
            query = query.OrderByDescending(specification.OrderByDescending);

        // Gruplama
        if (specification.GroupBy != null)
            query = query.GroupBy(specification.GroupBy).SelectMany(x => x);

        // Sayfalama
        if (specification.IsPagingEnabled)
            query = query.Skip(specification.Skip).Take(specification.Take);

        return query;
    }
}