namespace Domain.Common.Exceptions;

/// <summary>
/// Domain katmanı için temel exception sınıfı.
/// Tüm domain-specific exception'lar bu sınıftan türetilmelidir.
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary>
    /// Exception'ın tipi
    /// </summary>
    public string Type => GetType().Name;

    /// <summary>
    /// Exception'ın oluştuğu zaman
    /// </summary>
    public DateTime OccurredOn { get; }

    protected DomainException(string message) 
        : base(message)
    {
        OccurredOn = DateTime.UtcNow;
    }

    protected DomainException(string message, Exception innerException) 
        : base(message, innerException)
    {
        OccurredOn = DateTime.UtcNow;
    }
}