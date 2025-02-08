using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.Extensions;

/// <summary>
/// Servis registrasyonu için extension methods
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Uygulama servislerini register eder
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, Assembly assembly)
    {
        // Service interface'lerini bul
        var serviceInterfaces = assembly.GetTypes()
            .Where(t => t.IsInterface && t.Name.EndsWith("Service"))
            .ToList();

        // Implementasyonları bul ve register et
        foreach (var serviceInterface in serviceInterfaces)
        {
            var implementation = assembly.GetTypes()
                .FirstOrDefault(t => t.IsClass 
                                     && !t.IsAbstract 
                                     && serviceInterface.IsAssignableFrom(t));

            if (implementation != null)
            {
                services.AddScoped(serviceInterface, implementation);
            }
        }

        return services;
    }
}