namespace Domain.Common.Attributes;

/// <summary>
/// Value Object karşılaştırmasında göz ardı edilecek property'leri işaretlemek için kullanılır.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class IgnoreMemberAttribute : Attribute
{
}