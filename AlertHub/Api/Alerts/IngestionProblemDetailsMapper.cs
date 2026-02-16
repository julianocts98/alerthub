using AlertHub.Api.Common;
using AlertHub.Application.Alerts.Ingestion;
using AlertHub.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace AlertHub.Api.Alerts;

public static class IngestionProblemDetailsMapper
{
    public static IActionResult ToActionResult(ResultError? error)
    {
        if (error is null)
            return ApiProblemDetails.Build(StatusCodes.Status400BadRequest, "Ingestion failed", "Unexpected application error.");

        if (error.Code == IngestionErrorCodes.UnsupportedContentType)
            return ApiProblemDetails.Build(StatusCodes.Status415UnsupportedMediaType, "Unsupported media type", error.Message);

        if (error.Code == IngestionErrorCodes.InvalidPayload || error.Code == IngestionErrorCodes.XmlSchemaInvalid)
            return ApiProblemDetails.Build(StatusCodes.Status400BadRequest, "Invalid alert payload", error.Message);

        return ApiProblemDetails.Build(StatusCodes.Status422UnprocessableEntity, "Ingestion validation failed", error.Message);
    }
}
