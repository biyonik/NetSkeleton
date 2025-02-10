using Domain.Common.BusinessRules;

namespace Domain.Authorization.Rules;

/// <summary>
/// Sistem permission'larının değiştirilememesi kuralı
/// </summary>
public class SystemPermissionCannotBeModifiedRule(Permission permission) : IBusinessRule
{
    public string RuleName => "SystemPermissionCannotBeModified";
    public string Details => "System permissions cannot be modified or deleted";

    public Task<bool> IsValid()
    {
        return Task.FromResult(!permission.IsSystemPermission);
    }
}