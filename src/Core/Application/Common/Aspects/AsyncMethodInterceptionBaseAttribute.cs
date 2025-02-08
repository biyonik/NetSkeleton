using Castle.DynamicProxy;

namespace Application.Common.Aspects;

/// <summary>
/// Asenkron metot interceptor'ları için temel sınıf
/// </summary>
public abstract class AsyncMethodInterceptionBaseAttribute : MethodInterceptionBaseAttribute
{
    protected override void Intercept(IInvocation invocation)
    {
        if (invocation.Method.ReturnType == typeof(Task))
        {
            InterceptAsync(invocation).Wait();
        }
        else if (invocation.Method.ReturnType.IsGenericType && 
                 invocation.Method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            InterceptAsyncWithResult(invocation).Wait();
        }
        else
        {
            base.Intercept(invocation);
        }
    }

    protected virtual async Task InterceptAsync(IInvocation invocation)
    {
        var success = true;
        OnBefore(invocation);
        try
        {
            invocation.Proceed();
            var task = (Task)invocation.ReturnValue;
            await task;
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

    protected virtual async Task InterceptAsyncWithResult(IInvocation invocation)
    {
        var success = true;
        OnBefore(invocation);
        try
        {
            invocation.Proceed();
            var task = invocation.ReturnValue as Task;
            await task!;
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
}