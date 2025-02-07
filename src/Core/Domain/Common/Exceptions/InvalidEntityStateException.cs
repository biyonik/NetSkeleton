namespace Domain.Common.Exceptions;

/// <summary>
/// Domain entity'lerinin geçersiz durumları için fırlatılan exception
/// </summary>
public class InvalidEntityStateException(string entityType, IEnumerable<string> errors)
    : DomainException($"Entity '{entityType}' has validation errors: {string.Join(", ", errors)}")
{
    /// <summary>
    /// Geçersiz entity'nin tipi
    /// </summary>
    public string EntityType { get; } = entityType;

    /// <summary>
    /// Validation hataları
    /// </summary>
    public IReadOnlyList<string> Errors { get; } = errors.ToList().AsReadOnly();
}
