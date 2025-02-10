using Domain.Authorization.Events;
using Domain.Common.Abstractions;

namespace Domain.Authorization;

/// <summary>
/// Permission Grant entity'si
/// Kullanıcı ve izin arasındaki ilişkiyi temsil eder
/// </summary>
public class PermissionGrant : BaseEntity<Guid>
{
    public string UserId { get; private set; }
    public Guid PermissionId { get; private set; }
    public Permission Permission { get; private set; }
    public string? Restrictions { get; private set; }
    public string? ValidFrom { get; private set; }
    public string? ValidTo { get; private set; }
    public bool IsActive { get; private set; } = true;

    protected PermissionGrant() { } // EF Core için

    public static PermissionGrant Create(
        string userId,
        Guid permissionId,
        string? restrictions = null,
        string? validFrom = null,
        string? validTo = null)
    {
        var grant = new PermissionGrant
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PermissionId = permissionId,
            Restrictions = restrictions,
            ValidFrom = validFrom,
            ValidTo = validTo
        };

        grant.AddDomainEvent(new PermissionGrantCreatedDomainEvent(grant.Id, userId, permissionId));
        return grant;
    }

    public void Update(string? restrictions, string? validFrom, string? validTo)
    {
        Restrictions = restrictions;
        ValidFrom = validFrom;
        ValidTo = validTo;

        AddDomainEvent(new PermissionGrantUpdatedDomainEvent(Id, UserId, PermissionId));
    }

    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
        AddDomainEvent(new PermissionGrantDeactivatedDomainEvent(Id, UserId, PermissionId));
    }
}