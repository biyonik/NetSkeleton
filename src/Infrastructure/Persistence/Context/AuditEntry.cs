using System.Text.Json;
using Domain.Common.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Persistence.Context;

// <summary>
/// Entity değişikliklerini audit log'a çevirmek için yardımcı sınıf
/// </summary>
public class AuditEntry
{
    public EntityEntry Entry { get; }
    public string TableName { get; set; }
    public string UserId { get; set; }
    public Dictionary<string, object?> KeyValues { get; } = new();
    public Dictionary<string, object?> OldValues { get; } = new();
    public Dictionary<string, object?> NewValues { get; } = new();
    public List<PropertyEntry> TemporaryProperties { get; } = new();
    public List<string> ChangedColumns { get; } = new();

    public AuditEntry(EntityEntry entry)
    {
        Entry = entry;
    }

    /// <summary>
    /// AuditEntry'i AuditTrail'e çevirir
    /// </summary>
    public AuditTrail ToAudit()
    {
        var auditType = Entry.State switch
        {
            EntityState.Added => "Created",
            EntityState.Deleted => "Deleted",
            EntityState.Modified => "Updated",
            _ => "Unknown"
        };

        var audit = AuditTrail.Create(
            auditType,
            TableName,
            JsonSerializer.Serialize(KeyValues),
            OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues),
            NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues),
            ChangedColumns.Count == 0 ? null : JsonSerializer.Serialize(ChangedColumns),
            UserId);

        return audit;
    }
}