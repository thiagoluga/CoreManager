using Luga.BuildingBlocks.Domain.Common;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Luga.BuildingBlocks.Server.Http;

/// <summary>
/// Translates <see cref="Result"/> / <see cref="Result{T}"/> into HTTP
/// <c>IActionResult</c> values shaped as RFC 7807 <see cref="ProblemDetails"/>
/// (CLAUDE.md §7.15). Lives in <c>BuildingBlocks.Server</c> because
/// <c>BuildingBlocks.Application</c> must stay free of ASP.NET Core types.
/// </summary>
/// <remarks>
/// Error code → HTTP status mapping uses the conventional <c>{Entity}.{Reason}</c>
/// suffix (e.g. <c>Customer.NotFound</c> → 404, <c>General.Conflict</c> → 409).
/// Unknown codes default to 400.
/// </remarks>
public static class ResultExtensions
{
    /// <summary>
    /// Returns <c>200 OK</c> with the success value, or a <c>ProblemDetails</c>
    /// shaped response with the appropriate status code on failure.
    /// </summary>
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsSuccess)
        {
            return new OkObjectResult(result.Value);
        }

        return BuildProblem(result.Error);
    }

    /// <summary>
    /// Returns <c>204 No Content</c> on success, or a <c>ProblemDetails</c>
    /// shaped response with the appropriate status code on failure.
    /// </summary>
    public static IActionResult ToActionResult(this Result result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsSuccess)
        {
            return new NoContentResult();
        }

        return BuildProblem(result.Error);
    }

    /// <summary>
    /// Variant that returns <c>201 Created</c> with a Location header on success.
    /// </summary>
    public static IActionResult ToCreatedActionResult<T>(this Result<T> result, string location)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentException.ThrowIfNullOrWhiteSpace(location);

        if (result.IsSuccess)
        {
            return new CreatedResult(location, result.Value);
        }

        return BuildProblem(result.Error);
    }

    private static ObjectResult BuildProblem(Error error)
    {
        int status = MapStatusCode(error.Code);
        ProblemDetails problem = new()
        {
            Type = $"https://luga.com/errors/{error.Code}",
            Title = error.Code,
            Detail = error.Message,
            Status = status,
        };

        return new ObjectResult(problem)
        {
            StatusCode = status,
            ContentTypes = { "application/problem+json" },
        };
    }

    private static int MapStatusCode(string errorCode)
    {
        if (errorCode.EndsWith(".NotFound", StringComparison.Ordinal))
        {
            return StatusCodes.Status404NotFound;
        }

        if (errorCode.EndsWith(".Unauthorized", StringComparison.Ordinal))
        {
            return StatusCodes.Status401Unauthorized;
        }

        if (errorCode.EndsWith(".Forbidden", StringComparison.Ordinal))
        {
            return StatusCodes.Status403Forbidden;
        }

        if (errorCode.EndsWith(".Conflict", StringComparison.Ordinal))
        {
            return StatusCodes.Status409Conflict;
        }

        if (errorCode.EndsWith(".Validation", StringComparison.Ordinal))
        {
            return StatusCodes.Status422UnprocessableEntity;
        }

        if (errorCode.EndsWith(".Concurrency", StringComparison.Ordinal))
        {
            return StatusCodes.Status412PreconditionFailed;
        }

        if (errorCode.EndsWith(".Unexpected", StringComparison.Ordinal))
        {
            return StatusCodes.Status500InternalServerError;
        }

        return StatusCodes.Status400BadRequest;
    }
}
