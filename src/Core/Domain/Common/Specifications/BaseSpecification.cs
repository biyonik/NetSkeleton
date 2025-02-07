using System.Linq.Expressions;

namespace Domain.Common.Specifications;

/// <summary>
/// Tüm specification'lar için temel sınıf.
/// Bu sınıf, ISpecification interface'ini implemente eder ve temel fonksiyonellikleri sağlar.
/// </summary>
/// <typeparam name="T">Specification'ın uygulanacağı entity tipi</typeparam>
public abstract class BaseSpecification<T> : ISpecification<T>
{
    /// <summary>
    /// Specification'ın kriteri
    /// Protected set ile sadece türetilen sınıfların set etmesine izin veriyoruz
    /// </summary>
    public Expression<Func<T, bool>> Criteria { get; protected set; }
    
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }
    public Expression<Func<T, object>>? GroupBy { get; private set; }
    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    /// <summary>
    /// Default constructor - Tüm kayıtları getirir
    /// </summary>
    protected BaseSpecification()
    {
        Criteria = _ => true;
    }

    /// <summary>
    /// Belirli bir kriter ile specification oluşturur
    /// </summary>
    protected BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    /// <summary>
    /// Navigation property'leri include etmek için kullanılır
    /// </summary>
    protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    /// <summary>
    /// String olarak navigation property include etmek için kullanılır
    /// </summary>
    protected virtual void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    /// <summary>
    /// Sayfalama için kullanılır
    /// </summary>
    protected virtual void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    /// <summary>
    /// Sıralama için kullanılır
    /// </summary>
    protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    /// <summary>
    /// Tersten sıralama için kullanılır
    /// </summary>
    protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }

    /// <summary>
    /// Gruplama için kullanılır
    /// </summary>
    protected virtual void ApplyGroupBy(Expression<Func<T, object>> groupByExpression)
    {
        GroupBy = groupByExpression;
    }
}