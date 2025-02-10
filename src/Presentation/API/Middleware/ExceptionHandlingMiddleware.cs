using System.Net;
using Application.Common.Exceptions;
using Application.Common.Security.Exceptions;
using Domain.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace API.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ProblemDetails();

        switch (exception)
        {
            case SecurityTokenException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Status = (int)HttpStatusCode.Unauthorized;
                response.Title = "Unauthorized";
                response.Detail = exception.Message;
                response.Type = "https://tools.ietf.org/html/rfc7235#section-3.1";
                break;

            case ForbiddenAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                response.Status = (int)HttpStatusCode.Forbidden;
                response.Title = "Forbidden";
                response.Detail = exception.Message;
                response.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3";
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Status = (int)HttpStatusCode.Unauthorized;
                response.Title = "Unauthorized";
                response.Detail = "You are not authorized to access this resource";
                response.Type = "https://tools.ietf.org/html/rfc7235#section-3.1";
                break;

            case BusinessRuleValidationException businessException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Status = (int)HttpStatusCode.BadRequest;
                response.Title = "Business Rule Violation";
                response.Detail = businessException.Message;
                response.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Status = (int)HttpStatusCode.InternalServerError;
                response.Title = "Server Error";
                response.Detail = "An internal server error occurred.";
                response.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
                break;
        }

        await context.Response.WriteAsJsonAsync(response);
    }
}