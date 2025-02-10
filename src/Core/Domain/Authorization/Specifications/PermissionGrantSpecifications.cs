using Domain.Common.Specifications;

namespace Domain.Authorization.Specifications;

/// <summary>
/// Permission Grant Specification'ları
/// </summary>
public static class PermissionGrantSpecifications
{
    public sealed class ByUserIdSpec : BaseSpecification<PermissionGrant>
    {
        public ByUserIdSpec(string userId, bool includeInactive = false)
            : base(g => g.UserId == userId && (includeInactive || g.IsActive))
        {
            AddInclude(g => g.Permission);
        }
    }

    public sealed class ByPermissionIdSpec : BaseSpecification<PermissionGrant>
    {
        public ByPermissionIdSpec(Guid permissionId, bool includeInactive = false)
            : base(g => g.PermissionId == permissionId && (includeInactive || g.IsActive))
        {
            AddInclude(g => g.Permission);
        }
    }

    public sealed class ActiveGrantsForPermissionSpec : BaseSpecification<PermissionGrant>
    {
        public ActiveGrantsForPermissionSpec(string permissionSystemName)
            : base(g => g.Permission.SystemName == permissionSystemName && 
                       g.IsActive && 
                       g.Permission.IsActive)
        {
            AddInclude(g => g.Permission);
        }
    }

    public sealed class ValidGrantsSpec : BaseSpecification<PermissionGrant>
    {
        public ValidGrantsSpec(string userId, string permissionSystemName)
            : base(g => g.UserId == userId && 
                       g.Permission.SystemName == permissionSystemName && 
                       g.IsActive && 
                       g.Permission.IsActive)
        {
            AddInclude(g => g.Permission);
        }
    }

    public sealed class PagedGrantsSpec : BaseSpecification<PermissionGrant>
    {
        public PagedGrantsSpec(int pageIndex, int pageSize)
            : base(g => g.IsActive)
        {
            AddInclude(g => g.Permission);
            ApplyPaging(pageIndex * pageSize, pageSize);
            ApplyOrderByDescending(g => g.CreatedDate);
        }
    }

    public sealed class ExpiredGrantsSpec : BaseSpecification<PermissionGrant>
    {
        public ExpiredGrantsSpec()
            : base(g => g.IsActive && 
                       g.ValidTo != null && 
                       DateTime.Parse(g.ValidTo) < DateTime.UtcNow)
        {
            AddInclude(g => g.Permission);
        }
    }
}