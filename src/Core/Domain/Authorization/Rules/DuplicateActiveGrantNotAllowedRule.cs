using Domain.Authorization.Repositories;
using Domain.Common.BusinessRules;

namespace Domain.Authorization.Rules;


/// <summary>
/// Aynı permission için birden fazla aktif grant olmaması kuralı
/// </summary>
public class DuplicateActiveGrantNotAllowedRule(
    IAuthorizationRepository repository,
    string userId,
    Guid permissionId,
    Guid? excludeGrantId = null) : IBusinessRule
{
    public string RuleName => "DuplicateActiveGrantNotAllowed";
    public string Details => "User already has an active grant for this permission";

    public async Task<bool> IsValid()
    {
        var grants = await repository.GetPermissionGrantsAsync(permissionId, false);
        
        return !grants.Any(g => 
            g.UserId == userId && 
            g.IsActive && 
            (excludeGrantId == null || g.Id != excludeGrantId));
    }
}