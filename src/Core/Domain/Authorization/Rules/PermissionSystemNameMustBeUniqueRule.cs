using Domain.Authorization.Repositories;
using Domain.Common.BusinessRules;

namespace Domain.Authorization.Rules;

/// <summary>
/// Permission system name'inin benzersiz olması kuralı
/// </summary>
public class PermissionSystemNameMustBeUniqueRule(
    IAuthorizationRepository repository,
    string systemName,
    Guid? excludeId = null)
    : IBusinessRule
{
    public string RuleName => "PermissionSystemNameMustBeUnique";
    public string Details => $"A permission with system name '{systemName}' already exists";

    public async Task<bool> IsValid()
    {
        return !await repository.PermissionExistsAsync(systemName, excludeId);
    }
}