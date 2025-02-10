using Application.Authorization.Services;
using Application.Cache;
using Application.Common.Security.Authorization;
using Application.Common.Security.Handlers;
using Application.Common.Security.Services;
using Domain.Authorization.Repositories;
using Infrastructure.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;

namespace Infrastructure.Identity.Extensions;

public static class AuthorizationConfiguration
{
    public static IServiceCollection AddAuthorizationInfrastructure(
        this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Permission-based policies
            options.AddPolicy("RequirePermission", policy =>
                policy.RequireAuthenticatedUser()
                    .AddRequirements(new PermissionRequirement()));

            // Admin policies (isteğe bağlı, direkt permission'larla da yönetilebilir)
            options.AddPolicy("SuperAdmin", policy =>
                policy.RequireAuthenticatedUser()
                    .AddRequirements(new PermissionRequirement("System.SuperAdmin")));

            options.AddPolicy("SystemAdmin", policy =>
                policy.RequireAuthenticatedUser()
                    .AddRequirements(new PermissionRequirement("System.Admin")));
        });

        // Register handlers
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, ResourceAuthorizationHandler>();

        // Register services
        services.AddScoped<IJwtService, JwtService>();

        // Register cache
        services.AddMemoryCache();

        return services;
    }
}