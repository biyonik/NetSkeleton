using Domain.Common.Events;

namespace Domain.Authorization.Events;

public class PermissionClaimsUpdatedDomainEvent(Guid permissionId, string claims) : BaseDomainEvent("System")
{
    public Guid PermissionId { get; } = permissionId;
    public string Claims { get; } = claims;
}