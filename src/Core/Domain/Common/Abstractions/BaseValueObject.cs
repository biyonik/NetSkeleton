using System.Reflection;
using Domain.Common.Attributes;

namespace Domain.Common.Abstractions;

/// <summary>
/// Value Object'ler için temel sınıf.
/// Value Object'ler immutable'dır ve identity yerine değerlerine göre karşılaştırılır.
/// 
/// Örnek Kullanım:
/// public class Money : BaseValueObject
/// {
///     public decimal Amount { get; }
///     public string Currency { get; }
///     
///     public Money(decimal amount, string currency)
///     {
///         Amount = amount;
///         Currency = currency;
///     }
///     
///     protected override IEnumerable<object> GetEqualityComponents()
///     {
///         yield return Amount;
///         yield return Currency;
///     }
/// }
/// </summary>
public abstract class BaseValueObject : IEquatable<BaseValueObject>
{
    private List<PropertyInfo>? _properties;
    private List<FieldInfo>? _fields;

    /// <summary>
    /// Value Object'in eşitlik karşılaştırması için kullanılacak property'leri döner.
    /// Bu metod, türetilen sınıflarda override edilmelidir.
    /// </summary>
    protected abstract IEnumerable<object> GetEqualityComponents();

    /// <summary>
    /// Value Object'in property'lerini reflection ile alır.
    /// Performans optimizasyonu için caching yapılır.
    /// </summary>
    private void GetProperties()
    {
        _properties = GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.GetCustomAttribute<IgnoreMemberAttribute>() == null)
            .ToList();

        _fields = GetType()
            .GetFields(BindingFlags.Instance | BindingFlags.Public)
            .Where(f => f.GetCustomAttribute<IgnoreMemberAttribute>() == null)
            .ToList();
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (BaseValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }

    public bool Equals(BaseValueObject? other)
    {
        return Equals((object?)other);
    }

    public static bool operator ==(BaseValueObject? left, BaseValueObject? right)
    {
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            return true;

        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(BaseValueObject? left, BaseValueObject? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Value Object'ler için copy oluşturur.
    /// Immutability prensibini korumak için kullanılır.
    /// </summary>
    public T GetCopy<T>() where T : BaseValueObject
    {
        if (_properties == null || _fields == null)
        {
            GetProperties();
        }

        var constructor = GetType().GetConstructor(
            (Type[])_properties!.Select<PropertyInfo, object>(p => p.PropertyType).ToArray());

        if (constructor == null)
        {
            throw new ArgumentException(
                $"No matching constructor for {GetType().Name}.");
        }

        var parameters = _properties!.Select(p => p.GetValue(this)).ToArray();
        var copy = (T)constructor.Invoke(parameters);

        return copy;
    }
}