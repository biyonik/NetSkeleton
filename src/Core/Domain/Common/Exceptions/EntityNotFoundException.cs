namespace Domain.Common.Exceptions;

/// <summary>
/// Entity bulunamadığında fırlatılan exception
/// </summary>
public class EntityNotFoundException(string entityType, object entityId)
    : DomainException($"Entity '{entityType}' with id '{entityId}' was not found.")
{
    /// <summary>
    /// Bulunamayan entity'nin tipi
    /// </summary>
    public string EntityType { get; } = entityType;

    /// <summary>
    /// Aranan ID değeri
    /// </summary>
    public object EntityId { get; } = entityId;
}