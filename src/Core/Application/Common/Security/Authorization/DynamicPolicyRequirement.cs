using Microsoft.AspNetCore.Authorization;

namespace Application.Common.Security.Authorization;

/// <summary>
/// Dinamik policy requirement'ı
/// </summary>
public class DynamicPolicyRequirement(string policyName) : IAuthorizationRequirement
{
    /// <summary>
    /// Policy adı
    /// </summary>
    public string PolicyName { get; } = policyName;
}
