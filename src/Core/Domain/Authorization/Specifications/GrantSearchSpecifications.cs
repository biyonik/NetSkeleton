using Domain.Common.Specifications;

namespace Domain.Authorization.Specifications;

/// <summary>
/// Permission Grant arama ve listeleme için composite specification'lar
/// </summary>
public static class GrantSearchSpecifications
{
    public sealed class SearchGrantsSpec : BaseSpecification<PermissionGrant>
    {
        public SearchGrantsSpec(
            string? userId = null,
            Guid? permissionId = null,
            bool includeInactive = false,
            int? skip = null,
            int? take = null)
        {
            var baseCriteria = PredicateBuilder.True<PermissionGrant>();


            if (!string.IsNullOrWhiteSpace(userId))
            {
                baseCriteria = ExpressionHelper.And(baseCriteria, g => g.UserId == userId);
            }

            if (permissionId.HasValue)
            {
                baseCriteria = ExpressionHelper.And(baseCriteria, g => g.PermissionId == permissionId.Value);
            }

            if (!includeInactive)
            {
                baseCriteria = ExpressionHelper.And(baseCriteria, g => g.IsActive);
            }

            Criteria = baseCriteria;
            AddInclude(g => g.Permission);

            if (skip.HasValue && take.HasValue)
            {
                ApplyPaging(skip.Value, take.Value);
            }

            ApplyOrderByDescending(g => g.CreatedDate);
        }
    }
}