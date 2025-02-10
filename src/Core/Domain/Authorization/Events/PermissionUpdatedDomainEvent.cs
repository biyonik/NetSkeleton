using Domain.Common.Events;

namespace Domain.Authorization.Events;

public class PermissionUpdatedDomainEvent(Guid permissionId, string systemName) : BaseDomainEvent("System")
{
    public Guid PermissionId { get; } = permissionId;
    public string SystemName { get; } = systemName;
}