using AlertHub.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace AlertHub.Api.Common;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return new NoContentResult();
        }

        return MapError(result.Error);
    }

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(result.Value);
        }

        return MapError(result.Error);
    }

    public static IActionResult ToActionResult<T>(this Result<T> result, Func<T?, IActionResult> successMapper)
    {
        if (result.IsSuccess)
        {
            return successMapper(result.Value);
        }

        return MapError(result.Error);
    }

    private static IActionResult MapError(ResultError? error)
    {
        if (error is null)
        {
            return ApiProblemDetails.Build(
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred.");
        }

        var statusCode = error.Code switch
        {
            var c when c.EndsWith(".not_found") => StatusCodes.Status404NotFound,
            var c when c.EndsWith(".unauthorized") => StatusCodes.Status401Unauthorized,
            var c when c.EndsWith(".forbidden") => StatusCodes.Status403Forbidden,
            var c when c.EndsWith(".conflict") => StatusCodes.Status409Conflict,
            "ingestion.content_type.unsupported" => StatusCodes.Status415UnsupportedMediaType,
            var c when c.StartsWith("alert.") => StatusCodes.Status422UnprocessableEntity,
            var c when c.StartsWith("subscription.") => StatusCodes.Status422UnprocessableEntity,
            _ => StatusCodes.Status400BadRequest
        };

        var title = GetTitleForStatus(statusCode);

        return ApiProblemDetails.Build(statusCode, title, error.Message);
    }

    private static string GetTitleForStatus(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest => "Bad Request",
        StatusCodes.Status401Unauthorized => "Unauthorized",
        StatusCodes.Status403Forbidden => "Forbidden",
        StatusCodes.Status404NotFound => "Not Found",
        StatusCodes.Status409Conflict => "Conflict",
        StatusCodes.Status415UnsupportedMediaType => "Unsupported Media Type",
        StatusCodes.Status422UnprocessableEntity => "Unprocessable Entity",
        _ => "Error"
    };
}
