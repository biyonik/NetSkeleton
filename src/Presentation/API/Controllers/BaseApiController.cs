using Application.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Tüm controller'lar için temel sınıf
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private ISender? _mediator;
    
    /// <summary>
    /// MediatR sender
    /// </summary>
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    /// <summary>
    /// Result'ı IActionResult'a dönüştürür
    /// </summary>
    protected IActionResult FromResult(Result result)
    {
        if (result.IsSuccess)
            return Ok(result);

        return result.Error.Code switch
        {
            "Error.NotFound" => NotFound(result),
            "Error.Validation" => BadRequest(result),
            "Error.Unauthorized" => Unauthorized(result),
            "Error.Forbidden" => StatusCode(StatusCodes.Status403Forbidden, result),
            "Error.Conflict" => Conflict(result),
            _ => BadRequest(result)
        };
    }

    /// <summary>
    /// Generic Result'ı IActionResult'a dönüştürür
    /// </summary>
    protected IActionResult FromResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result);

        return result.Error.Code switch
        {
            "Error.NotFound" => NotFound(result),
            "Error.Validation" => BadRequest(result),
            "Error.Unauthorized" => Unauthorized(result),
            "Error.Forbidden" => StatusCode(StatusCodes.Status403Forbidden, result),
            "Error.Conflict" => Conflict(result),
            _ => BadRequest(result)
        };
    }

    /// <summary>
    /// PagedResult'ı IActionResult'a dönüştürür
    /// </summary>
    protected IActionResult FromResult<T>(PagedResult<T> result)
    {
        if (result.IsSuccess)
            return Ok(result);

        return result.Error.Code switch
        {
            "Error.NotFound" => NotFound(result),
            "Error.Validation" => BadRequest(result),
            "Error.Unauthorized" => Unauthorized(result),
            "Error.Forbidden" => StatusCode(StatusCodes.Status403Forbidden, result),
            "Error.Conflict" => Conflict(result),
            _ => BadRequest(result)
        };
    }

    /// <summary>
    /// Created response döner
    /// </summary>
    protected IActionResult Created<T>(Result<T> result, string? routeName = null, object? routeValues = null)
    {
        if (!result.IsSuccess)
            return FromResult(result);

        if (routeName != null)
            return CreatedAtRoute(routeName, routeValues, result);

        return StatusCode(StatusCodes.Status201Created, result);
    }
    

    /// <summary>
    /// No content response döner
    /// </summary>
    protected IActionResult NoContent(Result result)
    {
        if (!result.IsSuccess)
            return FromResult(result);

        return StatusCode(StatusCodes.Status204NoContent);
    }
}