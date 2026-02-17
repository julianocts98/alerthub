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

        var statusCode = ResolveStatusCode(error.Kind);

        var title = GetTitleForStatus(statusCode);

        return ApiProblemDetails.Build(statusCode, title, error.Message);
    }

    private static int ResolveStatusCode(ErrorKind kind) => kind switch
    {
        ErrorKind.Validation => StatusCodes.Status422UnprocessableEntity,
        ErrorKind.NotFound => StatusCodes.Status404NotFound,
        ErrorKind.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorKind.Conflict => StatusCodes.Status409Conflict,
        ErrorKind.BadRequest => StatusCodes.Status400BadRequest,
        ErrorKind.UnsupportedMediaType => StatusCodes.Status415UnsupportedMediaType,
        ErrorKind.Unexpected => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status500InternalServerError
    };

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
