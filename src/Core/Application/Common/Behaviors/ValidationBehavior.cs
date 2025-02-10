using Application.Common.Results;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

/// <summary>
/// Validation pipeline behavior'u
/// </summary>
public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse?> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        logger.LogWarning(
            "Validation failed for {RequestType}. Errors: {@ValidationErrors}",
            typeof(TRequest).Name, 
            failures);

        if (typeof(TResponse).IsGenericType && 
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var type = typeof(TResponse).GetGenericArguments()[0];
    
            var validationErrors = failures
                .GroupBy(f => f.PropertyName)
                .ToDictionary(
                    g => g.Key, 
                    g => g.Select(f => f.ErrorMessage).ToList()
                );
    
            var error = Error.Validation(validationErrors);
    
            return (TResponse)typeof(Result)
                .GetMethod(nameof(Result.Failure), 1, new[] { typeof(Error) })
                .MakeGenericMethod(type)
                .Invoke(null, new object[] { error });
        }

        // Normal Result tipinde ise
        if (typeof(TResponse) == typeof(Result))
        {
            var validationErrors = failures
                .GroupBy(f => f.PropertyName)
                .ToDictionary(
                    g => g.Key, 
                    g => g.Select(f => f.ErrorMessage).ToList()
                );
   
            var error = Error.Validation(validationErrors);
            return (TResponse)(object)Result.Failure(error);
        }

        throw new ValidationException("Validation failed", failures);
    }
}