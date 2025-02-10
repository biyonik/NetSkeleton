using Application.Authorization.Services;
using Application.Common.Security.Exceptions;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.Aspects;

/// <summary>
/// Method seviyesinde yetkilendirme için aspect
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AuthorizeOperationAttribute : AsyncMethodInterceptionBaseAttribute
{
    private readonly string[] _permissions;
    private readonly bool _requiresAll;

    public AuthorizeOperationAttribute(params string[] permissions)
        : this(true, permissions)
    {
    }

    public AuthorizeOperationAttribute(bool requiresAll, params string[] permissions)
    {
        _permissions = permissions;
        _requiresAll = requiresAll;
        Priority = 1; // Öncelik sırası
    }

    protected override async Task InterceptAsync(IInvocation invocation)
    {
        var currentUserService = ServiceTool.ServiceProvider
            .GetService<ICurrentUserService>();

        if (currentUserService == null)
            throw new InvalidOperationException("ICurrentUserService is not registered");

        // Super admin kontrolü
        if (currentUserService.IsInRole("SuperAdmin"))
        {
            await base.InterceptAsync(invocation);
            return;
        }

        // Permission kontrolü
        var hasPermission = _requiresAll
            ? _permissions.All(p => currentUserService.HasPermission(p))
            : _permissions.Any(p => currentUserService.HasPermission(p));

        if (!hasPermission)
            throw new ForbiddenAccessException();

        await base.InterceptAsync(invocation);
    }
}