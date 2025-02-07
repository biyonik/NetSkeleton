using System.Linq.Expressions;

namespace Domain.Common.Specifications;

/// <summary>
/// Specification pattern için temel interface.
/// Bu interface, bir entity'nin belirli bir kriteri sağlayıp sağlamadığını kontrol eder.
/// </summary>
/// <typeparam name="T">Specification'ın uygulanacağı entity tipi</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Kriterleri içeren expression
    /// </summary>
    Expression<Func<T, bool>> Criteria { get; }
    
    /// <summary>
    /// Include edilecek navigation property'ler
    /// </summary>
    List<Expression<Func<T, object>>> Includes { get; }
    
    /// <summary>
    /// String olarak include edilecek navigation property'ler
    /// </summary>
    List<string> IncludeStrings { get; }
    
    /// <summary>
    /// Sıralama için kullanılacak expression
    /// </summary>
    Expression<Func<T, object>>? OrderBy { get; }
    
    /// <summary>
    /// Tersten sıralama için kullanılacak expression
    /// </summary>
    Expression<Func<T, object>>? OrderByDescending { get; }
    
    /// <summary>
    /// Grup by operasyonu için kullanılacak expression
    /// </summary>
    Expression<Func<T, object>>? GroupBy { get; }

    /// <summary>
    /// Sayfalama - Kaç kayıt atlanacak
    /// </summary>
    int Take { get; }
    
    /// <summary>
    /// Sayfalama - Kaç kayıt alınacak
    /// </summary>
    int Skip { get; }
    
    /// <summary>
    /// Sayfalama yapılıp yapılmayacağı
    /// </summary>
    bool IsPagingEnabled { get; }
}