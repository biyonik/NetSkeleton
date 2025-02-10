using Application.Authorization.EventHandlers;
using Application.Authorization.Services;
using Application.Common.Security.Handlers;
using Domain.Authorization.Events;
using Domain.Authorization.Repositories;
using Domain.Common.Events;
using Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;
using IAuthorizationService = Application.Authorization.Services.IAuthorizationService;

namespace Infrastructure.DependencyInjection;

public static class AuthorizationServiceCollectionExtensions
{
    public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IAuthorizationRepository, AuthorizationRepository>();

        // Event Handlers
        services.AddScoped<IDomainEventHandler<PermissionCreatedDomainEvent>, PermissionCreatedDomainEventHandler>();
        services.AddScoped<IDomainEventHandler<PermissionUpdatedDomainEvent>, PermissionUpdatedDomainEventHandler>();
        services.AddScoped<IDomainEventHandler<PermissionClaimsUpdatedDomainEvent>, PermissionClaimsUpdatedDomainEventHandler>();
        
        services.AddScoped<IDomainEventHandler<PermissionGrantCreatedDomainEvent>, PermissionGrantCreatedDomainEventHandler>();
        services.AddScoped<IDomainEventHandler<PermissionGrantUpdatedDomainEvent>, PermissionGrantUpdatedDomainEventHandler>();
        services.AddScoped<IDomainEventHandler<PermissionGrantDeactivatedDomainEvent>, PermissionGrantDeactivatedDomainEventHandler>();

        // Authorization Services
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        
        // Current user service'i register et
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, DynamicPermissionPolicyProvider>();

        services.AddAuthorization();


        // Optional: Add memory cache if not already registered
        services.AddMemoryCache();

        return services;
    }

    public static IServiceCollection AddAuthorizationWithCustomCache(
        this IServiceCollection services,
        Action<AuthorizationCacheOptions> configureOptions)
    {
        var options = new AuthorizationCacheOptions();
        configureOptions(options);

        // Base services
        services.AddAuthorizationServices();

        // Configure cache options
        services.Configure<AuthorizationCacheOptions>(opt =>
        {
            opt.DefaultExpirationMinutes = options.DefaultExpirationMinutes;
            opt.UserPermissionsCacheEnabled = options.UserPermissionsCacheEnabled;
            opt.ResourceAccessCacheEnabled = options.ResourceAccessCacheEnabled;
        });

        return services;
    }
}

public class AuthorizationCacheOptions
{
    public int DefaultExpirationMinutes { get; set; } = 30;
    public bool UserPermissionsCacheEnabled { get; set; } = true;
    public bool ResourceAccessCacheEnabled { get; set; } = true;
}