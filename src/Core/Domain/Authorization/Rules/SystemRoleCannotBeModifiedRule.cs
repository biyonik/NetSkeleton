using Domain.Common.BusinessRules;

namespace Domain.Authorization.Rules;

/// <summary>
/// Sistem rollerinin değiştirilememesi kuralı
/// </summary>
public class SystemRoleCannotBeModifiedRule(bool isSystemRole) : IBusinessRule
{
    public string RuleName => "SystemRoleCannotBeModified";
    public string Details => "System roles cannot be modified";

    public Task<bool> IsValid()
    {
        return Task.FromResult(!isSystemRole);
    }
}