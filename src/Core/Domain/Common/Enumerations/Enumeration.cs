using System.Reflection;

namespace Domain.Common.Enumerations;

/// <summary>
/// Enumeration pattern için temel sınıf.
/// Bu pattern, enum'ların davranış ve veri içermesini sağlar.
/// Normal enum'lardan farklı olarak, inheritance ve polymorphism destekler.
/// </summary>
public abstract class Enumeration(int id, string name) : IComparable
{
    /// <summary>
    /// Enumeration değerinin adı
    /// </summary>
    public string Name { get; private set; } = name;

    /// <summary>
    /// Enumeration değerinin sayısal karşılığı
    /// </summary>
    public int Id { get; private set; } = id;

    /// <summary>
    /// Verilen tip için tüm enumeration değerlerini döner
    /// </summary>
    public static IEnumerable<T> GetAll<T>() where T : Enumeration
    {
        return typeof(T)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();
    }

    /// <summary>
    /// ID'ye göre enumeration değeri bulur
    /// </summary>
    public static T FromId<T>(int id) where T : Enumeration
    {
        var matchingItem = GetAll<T>().FirstOrDefault(item => item.Id == id);
        
        if (matchingItem == null)
            throw new InvalidOperationException($"'{id}' is not a valid ID in {typeof(T)}");
        
        return matchingItem;
    }

    /// <summary>
    /// İsme göre enumeration değeri bulur
    /// </summary>
    public static T FromName<T>(string name) where T : Enumeration
    {
        var matchingItem = GetAll<T>().FirstOrDefault(item => 
            string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
        
        if (matchingItem == null)
            throw new InvalidOperationException($"'{name}' is not a valid name in {typeof(T)}");
        
        return matchingItem;
    }

    /// <summary>
    /// Verilen ID'nin geçerli olup olmadığını kontrol eder
    /// </summary>
    public static bool IsValidId<T>(int id) where T : Enumeration
    {
        return GetAll<T>().Any(item => item.Id == id);
    }

    /// <summary>
    /// Verilen ismin geçerli olup olmadığını kontrol eder
    /// </summary>
    public static bool IsValidName<T>(string name) where T : Enumeration
    {
        return GetAll<T>().Any(item => 
            string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Absolute değer farkını hesaplar
    /// </summary>
    public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
    {
        return Math.Abs(firstValue.Id - secondValue.Id);
    }

    public override string ToString() => Name;

    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration otherValue)
            return false;

        var typeMatches = GetType() == obj.GetType();
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public int CompareTo(object? other) => Id.CompareTo(((Enumeration)other!).Id);

    public static bool operator ==(Enumeration? left, Enumeration? right)
    {
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            return true;

        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(Enumeration? left, Enumeration? right)
    {
        return !(left == right);
    }

    public static bool operator <(Enumeration? left, Enumeration? right)
    {
        return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
    }

    public static bool operator <=(Enumeration? left, Enumeration? right)
    {
        return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
    }

    public static bool operator >(Enumeration? left, Enumeration? right)
    {
        return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
    }

    public static bool operator >=(Enumeration? left, Enumeration? right)
    {
        return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
    }
}