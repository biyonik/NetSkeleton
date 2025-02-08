using System.Reflection;
using Application.Common.Behaviors;
using Application.Common.CQRS.Dispatcher;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.Extensions;

/// <summary>
/// CQRS servislerinin registrasyonu için extension methods
/// </summary>
public static class CQRSExtensions
{
    /// <summary>
    /// CQRS servislerini ve pipeline behavior'larını register eder
    /// </summary>
    public static IServiceCollection AddCQRSServices(this IServiceCollection services, Assembly assembly)
    {
        // MediatR'ı register et
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);

            // Pipeline behavior'ları register et
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        });

        // Validator'ları register et
        services.AddValidatorsFromAssembly(assembly);

        // CQRS Dispatcher'ı register et
        services.AddScoped<ICQRSDispatcher, CQRSDispatcher>();

        return services;
    }
}