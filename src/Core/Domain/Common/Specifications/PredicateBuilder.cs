using System.Linq.Expressions;

namespace Domain.Common.Specifications;

/// <summary>
/// Predicate'leri dinamik olarak birleştirmek için yardımcı sınıf
/// </summary>
public static class PredicateBuilder
{
    /// <summary>
    /// True ile başlayan bir predicate oluşturur
    /// </summary>
    public static Expression<Func<T, bool>> True<T>() => _ => true;

    /// <summary>
    /// False ile başlayan bir predicate oluşturur
    /// </summary>
    public static Expression<Func<T, bool>> False<T>() => _ => false;

    /// <summary>
    /// İki predicate'i AND operatörü ile birleştirir
    /// </summary>
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr1.Body);

        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body);

        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(left!, right!), parameter);
    }

    /// <summary>
    /// İki predicate'i OR operatörü ile birleştirir
    /// </summary>
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr1.Body);

        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body);

        return Expression.Lambda<Func<T, bool>>(
            Expression.OrElse(left!, right!), parameter);
    }
}