using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.Aspects;

/// <summary>
/// Aspect'lerden servis erişimi için yardımcı sınıf
/// </summary>
public static class ServiceTool
{
    public static IServiceProvider ServiceProvider { get; private set; }

    public static IServiceCollection Create(IServiceCollection services)
    {
        ServiceProvider = services.BuildServiceProvider();
        return services;
    }
}