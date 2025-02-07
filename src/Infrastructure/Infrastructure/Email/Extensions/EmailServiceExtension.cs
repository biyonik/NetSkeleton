using Infrastructure.Email.Factory;
using Infrastructure.Email.Settings;
using Infrastructure.Email.Templates;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Email.Extensions;

/// <summary>
/// Email servislerinin registrasyonu için extension methods
/// </summary>
public static class EmailServiceExtensions
{
    /// <summary>
    /// Email servislerini DI container'a register eder
    /// </summary>
    public static IServiceCollection AddEmailServices(
        this IServiceCollection services,
        Action<EmailSettings> configureOptions)
    {
        // Settings'i configure et
        services.Configure(configureOptions);

        // Factory ve Manager'ı register et
        services.AddSingleton<IEmailStrategyFactory, EmailStrategyFactory>();
        services.AddSingleton<IEmailManager, EmailManager>();

        // Template renderer'ı register et
        services.AddScoped<ITemplateRenderer, RazorTemplateRenderer>();

        // Razor view engine için gerekli servisler
        services.AddControllersWithViews();
        services.AddRazorPages();

        return services;
    }
}