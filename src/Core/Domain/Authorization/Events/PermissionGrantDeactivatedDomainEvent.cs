using Domain.Common.Events;

namespace Domain.Authorization.Events;

public class PermissionGrantDeactivatedDomainEvent(Guid grantId, string userId, Guid permissionId)
    : BaseDomainEvent("System")
{
    public Guid GrantId { get; } = grantId;
    public string UserId { get; } = userId;
    public Guid PermissionId { get; } = permissionId;
}