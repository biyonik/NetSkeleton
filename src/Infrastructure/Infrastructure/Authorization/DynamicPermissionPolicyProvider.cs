using Application.Common.Security.Authorization;
using Domain.Authorization.Repositories;
using Infrastructure.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Authorization;

public class DynamicPermissionPolicyProvider(
    ICacheManager cacheManager,
    IServiceProvider serviceProvider)
    : IAuthorizationPolicyProvider
{
    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        using var scope = serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAuthorizationRepository>();

        // Policy cache kontrolü ve oluşturma
        var cacheKey = $"auth_policy:{policyName}";
        var policy = await cacheManager.GetAsync<AuthorizationPolicy>(cacheKey);
        
        if (policy != null)
            return policy;

        if (policyName.StartsWith("Permission_"))
        {
            var permissionName = policyName["Permission_".Length..];
            var permission = await repository.GetPermissionBySystemNameAsync(permissionName);

            if (permission != null)
            {
                policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new PermissionRequirement(permissionName))
                    .Build();

                await cacheManager.SetAsync(cacheKey, policy, TimeSpan.FromMinutes(30));
                return policy;
            }
        }

        return null;
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => 
        Task.FromResult(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => 
        Task.FromResult<AuthorizationPolicy?>(null);
}