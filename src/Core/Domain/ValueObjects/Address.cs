using Domain.Common.Abstractions;

namespace Domain.ValueObjects;

/// <summary>
/// Adres bilgilerini tutan Value Object.
/// Value Object olduğu için immutable'dır ve identity yerine değerlerine göre karşılaştırılır.
/// </summary>
public class Address : BaseValueObject
{
    /// <summary>
    /// Sokak adresi
    /// </summary>
    public string Street { get; }

    /// <summary>
    /// Şehir
    /// </summary>
    public string City { get; }

    /// <summary>
    /// Ülke
    /// </summary>
    public string Country { get; }

    /// <summary>
    /// Posta kodu
    /// </summary>
    public string ZipCode { get; }

    /// <summary>
    /// Adres oluşturmak için constructor.
    /// Tüm değerler constructor'da set edilir ve sonrasında değiştirilemez (immutable).
    /// </summary>
    public Address(string street, string city, string country, string zipCode)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty", nameof(street));
        
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));
        
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty", nameof(country));
        
        if (string.IsNullOrWhiteSpace(zipCode))
            throw new ArgumentException("ZipCode cannot be empty", nameof(zipCode));

        Street = street;
        City = city;
        Country = country;
        ZipCode = zipCode;
    }

    /// <summary>
    /// Value Object'in eşitlik karşılaştırması için kullanılacak property'leri döner.
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        // Sıralama önemli! Karşılaştırma bu sırayla yapılır
        yield return Street;
        yield return City;
        yield return Country;
        yield return ZipCode;
    }

    /// <summary>
    /// Adresin string temsilini döner
    /// </summary>
    public override string ToString()
    {
        return $"{Street}, {ZipCode} {City}, {Country}";
    }

    /// <summary>
    /// Factory method: Boş adres oluşturur
    /// </summary>
    public static Address Empty => new Address("", "", "", "");

    /// <summary>
    /// Factory method: Varolan bir adresten yeni bir adres oluşturur
    /// </summary>
    public Address WithStreet(string street)
    {
        return new Address(street, City, Country, ZipCode);
    }

    /// <summary>
    /// Factory method: Varolan bir adresten yeni bir adres oluşturur
    /// </summary>
    public Address WithCity(string city)
    {
        return new Address(Street, city, Country, ZipCode);
    }

    // Diğer WithXXX metotları da eklenebilir...
}