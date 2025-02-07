using Domain.Common.Abstractions;
using Domain.Common.Guards;
using Domain.Events;

namespace Domain.Common.Audit;

/// <summary>
/// Entity değişikliklerinin loglanması için kullanılan sınıf
/// </summary>
public class AuditTrail : BaseEntity<Guid>
{
    /// <summary>
    /// İşlem tipi (Created, Modified, Deleted, Restored)
    /// </summary>
    public string Operation { get; private set; }

    /// <summary>
    /// İşlemin yapıldığı tablo/entity adı
    /// </summary>
    public string TableName { get; private set; }

    /// <summary>
    /// İşlem yapılan kaydın ID'si
    /// </summary>
    public string RecordId { get; private set; }

    /// <summary>
    /// Değişiklikten önceki değerler (JSON formatında)
    /// </summary>
    public string? OldValues { get; private set; }

    /// <summary>
    /// Değişiklikten sonraki değerler (JSON formatında)
    /// </summary>
    public string? NewValues { get; private set; }

    /// <summary>
    /// Değişen alanların listesi
    /// </summary>
    public string? ChangedColumns { get; private set; }

    private AuditTrail() { } // EF Core için

    public static AuditTrail Create(
        string operation,
        string tableName,
        string recordId,
        string? oldValues,
        string? newValues,
        string? changedColumns,
        string userId)
    {
        Guard.AgainstNullOrEmpty(operation, nameof(operation));
        Guard.AgainstNullOrEmpty(tableName, nameof(tableName));
        Guard.AgainstNullOrEmpty(recordId, nameof(recordId));
        Guard.AgainstNullOrEmpty(userId, nameof(userId));

        var trail = new AuditTrail
        {
            Id = Guid.NewGuid(),
            Operation = operation,
            TableName = tableName,
            RecordId = recordId,
            OldValues = oldValues,
            NewValues = newValues,
            ChangedColumns = changedColumns,
            CreatedBy = userId
        };

        trail.AddDomainEvent(new AuditTrailCreatedDomainEvent(trail.Id, tableName, recordId, userId));
        
        return trail;
    }
}