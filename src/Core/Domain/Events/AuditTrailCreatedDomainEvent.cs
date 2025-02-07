using Domain.Common.Guards;
using MediatR;

namespace Domain.Events;

/// <summary>
/// Bir audit log kaydı oluşturulduğunda fırlatılan domain event
/// </summary>
public class AuditTrailCreatedDomainEvent : INotification
{
    /// <summary>
    /// Oluşturulan audit log kaydının ID'si
    /// </summary>
    public Guid AuditTrailId { get; }

    /// <summary>
    /// İşlemin yapıldığı tablo/entity adı
    /// </summary>
    public string TableName { get; }

    /// <summary>
    /// İşlem yapılan kaydın ID'si
    /// </summary>
    public string RecordId { get; }

    /// <summary>
    /// İşlemi yapan kullanıcı
    /// </summary>
    public string UserId { get; }

    /// <summary>
    /// Event'in oluşturulma zamanı
    /// </summary>
    public DateTime Timestamp { get; }

    public AuditTrailCreatedDomainEvent(
        Guid auditTrailId,
        string tableName,
        string recordId,
        string userId)
    {
        Guard.AgainstDefaultGuid(auditTrailId, nameof(auditTrailId));
        Guard.AgainstNullOrEmpty(tableName, nameof(tableName));
        Guard.AgainstNullOrEmpty(recordId, nameof(recordId));
        Guard.AgainstNullOrEmpty(userId, nameof(userId));

        AuditTrailId = auditTrailId;
        TableName = tableName;
        RecordId = recordId;
        UserId = userId;
        Timestamp = DateTime.UtcNow;
    }
}