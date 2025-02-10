using Domain.Common.Specifications;

namespace Domain.Authorization.Specifications;

/// <summary>
/// Permission Specification'ları
/// </summary>
public static class PermissionSpecifications
{
    public sealed class BySystemNameSpec(string systemName)
        : BaseSpecification<Permission>(p => p.SystemName == systemName);

    public sealed class ByCategorySpec(string category)
        : BaseSpecification<Permission>(p => p.Category == category && p.IsActive);

    public sealed class ActiveOnlySpec() : BaseSpecification<Permission>(p => p.IsActive);

    public sealed class WithGrantsSpec : BaseSpecification<Permission>
    {
        public WithGrantsSpec()
            : base(p => p.IsActive)
        {
            AddInclude(p => p.Grants);
        }
    }
}