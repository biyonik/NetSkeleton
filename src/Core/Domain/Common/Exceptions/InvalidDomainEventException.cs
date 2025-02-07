namespace Domain.Common.Exceptions;

/// <summary>
/// Geçersiz domain event durumlarında fırlatılan exception
/// </summary>
public class InvalidDomainEventException(string eventType, string message)
    : DomainException($"Invalid domain event '{eventType}': {message}")
{
    /// <summary>
    /// Event tipi
    /// </summary>
    public string EventType { get; } = eventType;
}