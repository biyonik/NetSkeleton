using Infrastructure.Identity.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Identity.Extensions;

/// <summary>
/// Identity servislerinin registrasyonu için extension methods
/// </summary>
public static class IdentityServiceExtension
{
    /// <summary>
    /// Identity ile ilgili servisleri DI container'a register eder
    /// </summary>
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        // HTTP context accessor'ı register et
        services.AddHttpContextAccessor();

        // Current user service'i register et
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}