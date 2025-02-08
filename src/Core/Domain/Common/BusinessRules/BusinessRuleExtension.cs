using Domain.Common.Exceptions;

namespace Domain.Common.BusinessRules;

/// <summary>
/// Business rule'ların kontrolü için extension
/// </summary>
public static class BusinessRuleExtension
{
    public static async Task CheckRule(this IBusinessRule rule)
    {
        if (!await rule.IsValid())
        {
            throw new BusinessRuleValidationException(rule.RuleName, rule.Details);
        }
    }
}
