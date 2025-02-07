using MediatR;

namespace Domain.Common.Events;

/// <summary>
/// Tüm domain event'ler için temel sınıf.
/// MediatR'ın INotification interface'ini implemente eder.
/// Bu sayede event'ler MediatR üzerinden publish edilebilir.
/// </summary>
public abstract class BaseDomainEvent(string triggeredBy) : INotification
{
    /// <summary>
    /// Event'in benzersiz tanımlayıcısı
    /// </summary>
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>
    /// Event'in oluşturulma zamanı
    /// </summary>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <summary>
    /// Event'i oluşturan kullanıcı
    /// </summary>
    public string TriggeredBy { get; } = triggeredBy;

    /// <summary>
    /// Event'in tipi (Event sınıfının adı)
    /// </summary>
    public string EventType => GetType().Name;
}