using Domain.Authorization.Repositories;
using Domain.Common.BusinessRules;

namespace Domain.Authorization.Rules;

/// <summary>
/// Gerekli claim'lerin kontrolü kuralı
/// </summary>
public class RequiredClaimsMustBePresentRule(
    IAuthorizationRepository repository,
    string userId,
    Permission permission) : IBusinessRule
{
    public string RuleName => "RequiredClaimsMustBePresent";
    public string Details => "User does not have all required claims for this permission";

    public async Task<bool> IsValid()
    {
        if (string.IsNullOrEmpty(permission.RequiredClaims))
            return true;

        var requiredClaims = permission.RequiredClaims
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(c => c.Trim());

        foreach (var claim in requiredClaims)
        {
            var (hasAccess, _) = await repository.EvaluateAccessAsync(userId, claim);
            if (!hasAccess)
                return false;
        }

        return true;
    }
}