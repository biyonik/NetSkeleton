using System.Reflection;
using Application.Common.Aspects;
using Application.Common.Validators;
using Autofac;
using Autofac.Extras.DynamicProxy;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        // Http context accessor
        services.AddHttpContextAccessor();

        // MediatR ve diğer servisler...
        services.AddScoped<IPasswordValidator, PasswordValidator>();


        
        // Service tool için service provider oluştur
        ServiceTool.Create(services);

        return services;
    }

    public static void AddApplicationModule(this ContainerBuilder builder)
    {
        // Service registrations
        // builder.RegisterType<ProductService>()
        //     .As<IProductService>()
        //     .EnableInterfaceInterceptors()
        //     .InterceptedBy(typeof(AuthorizeOperationAttribute))
        //     .InstancePerLifetimeScope();
        //
        // builder.RegisterType<OrderService>()
        //     .As<IOrderService>()
        //     .EnableInterfaceInterceptors()
        //     .InterceptedBy(typeof(AuthorizeOperationAttribute))
        //     .InstancePerLifetimeScope();

        // CQRS handlers
        var assembly = Assembly.GetExecutingAssembly();
        
        builder.RegisterAssemblyTypes(assembly)
            .AssignableTo<IBaseRequest>()
            .AsImplementedInterfaces()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(AuthorizeOperationAttribute))
            .InstancePerLifetimeScope();
    }
}