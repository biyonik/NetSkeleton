using System.Linq.Expressions;

namespace Domain.Common.Specifications;

/// <summary>
/// Expression parametrelerini değiştirmek için yardımcı sınıf
/// </summary>
internal class ReplaceExpressionVisitor(Expression oldValue, Expression newValue) : ExpressionVisitor
{
    public override Expression? Visit(Expression? node)
    {
        return node == oldValue ? newValue : base.Visit(node);
    }
}