using AlertHub.Application.Alerts.Ingestion;
using AlertHub.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace AlertHub.Api.Alerts;

public static class IngestionProblemDetailsMapper
{
    public static IActionResult ToActionResult(ResultError? error)
    {
        if (error is null)
            return Build(StatusCodes.Status400BadRequest, "Ingestion failed", "Unexpected application error.");

        if (error.Code == IngestionErrorCodes.UnsupportedContentType)
            return Build(StatusCodes.Status415UnsupportedMediaType, "Unsupported media type", error.Message);

        if (error.Code == IngestionErrorCodes.InvalidPayload || error.Code == IngestionErrorCodes.XmlSchemaInvalid)
            return Build(StatusCodes.Status400BadRequest, "Invalid alert payload", error.Message);

        return Build(StatusCodes.Status422UnprocessableEntity, "Ingestion validation failed", error.Message);
    }

    private static ObjectResult Build(int statusCode, string title, string detail)
    {
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        };

        return new ObjectResult(problem)
        {
            StatusCode = statusCode
        };
    }
}
