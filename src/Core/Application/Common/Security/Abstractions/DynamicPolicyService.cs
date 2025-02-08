using Infrastructure.Cache;
using Infrastructure.Identity.Services;
using Microsoft.Extensions.Logging;

namespace Application.Common.Security.Abstractions;

/// <summary>
/// Dinamik policy servis implementasyonu
/// </summary>
public class DynamicPolicyService(
    ICurrentUserService currentUserService,
    ICacheManager cacheManager,
    ILogger<DynamicPolicyService> logger)
    : IDynamicPolicyService
{
    public async Task<bool> EvaluatePolicyAsync(string policyName, string userId)
    {
        try
        {
            // Cache'den kontrol et
            var cacheKey = $"policy:{userId}:{policyName}";
            var cachedResult = await cacheManager.GetAsync<bool?>(cacheKey);
            
            if (cachedResult.HasValue)
                return cachedResult.Value;

            // Policy'i değerlendir
            var result = await EvaluatePolicyInternalAsync(policyName);

            // Cache'e kaydet (5 dakika)
            await cacheManager.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error evaluating policy {Policy} for user {UserId}", policyName, userId);
            return false;
        }
    }

    private async Task<bool> EvaluatePolicyInternalAsync(string policyName)
    {
        // Policy adına göre değerlendirme yap
        return policyName switch
        {
            // Örnek policy'ler
            "CanManageUsers" => currentUserService.HasPermission("Users.Manage"),
            "CanViewReports" => currentUserService.HasPermission("Reports.View"),
            "IsAdvancedUser" => currentUserService.IsInRole("AdvancedUser"),
            _ => false
        };
    }
}