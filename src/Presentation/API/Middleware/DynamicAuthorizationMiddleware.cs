using Domain.Authorization.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace API.Middleware;

public class DynamicAuthorizationMiddleware(
    RequestDelegate next,
    IServiceProvider serviceProvider)
{
    public async Task InvokeAsync(HttpContext context)
    {
        using var scope = serviceProvider.CreateScope();
        var authorizationService = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
        var authorizationRepository = scope.ServiceProvider.GetRequiredService<IAuthorizationRepository>();

        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            await next(context);
            return;
        }

        var routeData = context.GetRouteData();
        var controller = routeData?.Values["controller"]?.ToString();
        var action = routeData?.Values["action"]?.ToString();
        var method = context.Request.Method;

        if (controller != null && action != null)
        {
            var permissions = await authorizationRepository
                .GetPermissionsForEndpointAsync(controller, action, method);

            foreach (var permission in permissions)
            {
                var authResult = await authorizationService.AuthorizeAsync(
                    context.User,
                    null,
                    $"Permission_{permission.SystemName}");

                if (!authResult.Succeeded)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }
        }

        await next(context);
    }
}