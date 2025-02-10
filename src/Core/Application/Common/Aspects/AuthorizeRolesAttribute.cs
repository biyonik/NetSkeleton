using Application.Authorization.Services;
using Application.Common.Security.Exceptions;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.Aspects;

/// <summary>
/// Method seviyesinde rol bazlı yetkilendirme için aspect
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AuthorizeRolesAttribute : AsyncMethodInterceptionBaseAttribute
{
    private readonly string[] _roles;
    private readonly bool _requiresAll;

    public AuthorizeRolesAttribute(params string[] roles)
        : this(false, roles)
    {
    }

    public AuthorizeRolesAttribute(bool requiresAll, params string[] roles)
    {
        _roles = roles;
        _requiresAll = requiresAll;
        Priority = 1; // Öncelik sırası
    }

    protected override async Task InterceptAsync(IInvocation invocation)
    {
        var currentUserService = ServiceTool.ServiceProvider
            .GetService<ICurrentUserService>();

        if (currentUserService == null)
            throw new InvalidOperationException("ICurrentUserService is not registered");

        // Rol kontrolü
        var hasRole = _requiresAll
            ? _roles.All(r => currentUserService.IsInRole(r))
            : _roles.Any(r => currentUserService.IsInRole(r));

        if (!hasRole)
            throw new ForbiddenAccessException();

        await base.InterceptAsync(invocation);
    }
}