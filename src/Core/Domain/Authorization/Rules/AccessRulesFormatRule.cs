using System.Text.Json;
using Domain.Common.BusinessRules;

namespace Domain.Authorization.Rules;

/// <summary>
/// Access rules JSON formatı kontrolü
/// </summary>
public class AccessRulesFormatRule(string accessRules) : IBusinessRule
{
    public string RuleName => "InvalidAccessRulesFormat";
    public string Details => "Access rules must be in valid JSON format";

    public Task<bool> IsValid()
    {
        try
        {
            if (string.IsNullOrEmpty(accessRules))
                return Task.FromResult(true);

            JsonDocument.Parse(accessRules);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}