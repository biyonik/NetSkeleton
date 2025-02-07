namespace Domain.Common.Exceptions;

/// <summary>
/// Concurrent update durumlarında fırlatılan exception
/// </summary>
public class ConcurrencyException(string entityType, object entityId)
    : DomainException($"A different version of '{entityType}' with id '{entityId}' already exists.")
{
    /// <summary>
    /// Çakışan entity'nin tipi
    /// </summary>
    public string EntityType { get; } = entityType;

    /// <summary>
    /// Entity ID
    /// </summary>
    public object EntityId { get; } = entityId;
}