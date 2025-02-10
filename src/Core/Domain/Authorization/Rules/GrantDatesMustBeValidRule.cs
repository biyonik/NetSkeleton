using Domain.Common.BusinessRules;

namespace Domain.Authorization.Rules;

/// <summary>
/// Permission grant geçerlilik tarihlerinin doğruluğu kuralı
/// </summary>
public class GrantDatesMustBeValidRule(string? validFrom, string? validTo) : IBusinessRule
{
    public string RuleName => "GrantDatesMustBeValid";
    public string Details => "Grant dates are invalid";

    public Task<bool> IsValid()
    {
        if (string.IsNullOrEmpty(validFrom) || string.IsNullOrEmpty(validTo))
            return Task.FromResult(true);

        if (!DateTime.TryParse(validFrom, out var fromDate) || !DateTime.TryParse(validTo, out var toDate))
            return Task.FromResult(false);

        if (fromDate >= toDate)
            return Task.FromResult(false);

        if (toDate < DateTime.UtcNow)
            return Task.FromResult(false);

        return Task.FromResult(true);
    }
}