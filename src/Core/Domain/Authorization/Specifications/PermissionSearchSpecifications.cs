using Domain.Common.Specifications;

namespace Domain.Authorization.Specifications;

/// <summary>
/// Permission arama ve listeleme için composite specification'lar
/// </summary>
public static class PermissionSearchSpecifications
{
    public sealed class SearchPermissionsSpec : BaseSpecification<Permission>
    {
        public SearchPermissionsSpec(
            string? searchTerm = null,
            string? category = null,
            bool includeInactive = false,
            int? skip = null,
            int? take = null)
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchTermLower = searchTerm.ToLower();
                Criteria = p => (p.Name.ToLower().Contains(searchTermLower) ||
                                 p.SystemName.ToLower().Contains(searchTermLower) ||
                                 p.Description!.ToLower().Contains(searchTermLower)) &&
                                (includeInactive || p.IsActive);
            }
            else
            {
                Criteria = p => includeInactive || p.IsActive;
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                var categoryCriteria = ExpressionHelper.And(Criteria, p => p.Category == category);

                Criteria = categoryCriteria;
            }

            if (skip.HasValue && take.HasValue)
            {
                ApplyPaging(skip.Value, take.Value);
            }

            ApplyOrderBy(p => p.Name);
        }
    }
}