namespace Application.Common.Security.Abstractions;

/// <summary>
/// Dinamik policy servisi için interface
/// </summary>
public interface IDynamicPolicyService
{
    Task<bool> EvaluatePolicyAsync(string policyName, string userId);
}