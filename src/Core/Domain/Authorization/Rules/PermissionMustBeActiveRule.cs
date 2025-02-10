using Domain.Common.BusinessRules;

namespace Domain.Authorization.Rules;

/// <summary>
/// Permission'ın aktif olması kuralı
/// </summary>
public class PermissionMustBeActiveRule(Permission permission) : IBusinessRule
{
    public string RuleName => "PermissionMustBeActive";
    public string Details => "Cannot grant inactive permission";

    public Task<bool> IsValid()
    {
        return Task.FromResult(permission.IsActive);
    }
}