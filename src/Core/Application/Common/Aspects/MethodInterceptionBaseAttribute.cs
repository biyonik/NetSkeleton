using Castle.DynamicProxy;
using IInterceptor = Microsoft.EntityFrameworkCore.Diagnostics.IInterceptor;

namespace Application.Common.Aspects;

/// <summary>
/// Metot interceptor'ları için temel sınıf
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public abstract class MethodInterceptionBaseAttribute : Attribute, IInterceptor
{
    public int Priority { get; set; }

    protected virtual void Intercept(IInvocation invocation)
    {
        var success = true;
        OnBefore(invocation);
        try
        {
            invocation.Proceed();
        }
        catch (Exception e)
        {
            success = false;
            OnException(invocation, e);
            throw;
        }
        finally
        {
            if (success)
            {
                OnSuccess(invocation);
            }
        }
        OnAfter(invocation);
    }

    protected virtual void OnBefore(IInvocation invocation) { }
    protected virtual void OnAfter(IInvocation invocation) { }
    protected virtual void OnException(IInvocation invocation, Exception e) { }
    protected virtual void OnSuccess(IInvocation invocation) { }
}