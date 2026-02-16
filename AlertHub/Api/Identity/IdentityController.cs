using AlertHub.Api.Common;
using AlertHub.Application.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AlertHub.Api.Identity;

[ApiController]
[Route("api/identity")]
public sealed class IdentityController : ControllerBase
{
    private const string DemoIssuerHeaderName = "X-Demo-Issuer-Key";
    private readonly IdentityService _identityService;

    public IdentityController(IdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("token")]
    public IActionResult GenerateToken([FromBody] TokenRequest request)
    {
        var providedDemoKey = Request.Headers[DemoIssuerHeaderName].FirstOrDefault();
        var result = _identityService.IssueToken(
            new IssueTokenCommand(request.UserId, request.Role, request.Scopes),
            providedDemoKey);

        if (!result.IsSuccess || result.Value is null)
            return MapError(result.Error);

        return Ok(new { token = result.Value.Value });
    }

    private IActionResult MapError(Application.Common.ResultError? error)
    {
        if (error?.Code == IdentityErrorCodes.IssuerKeyNotConfigured)
            return ApiProblemDetails.Build(
                StatusCodes.Status404NotFound,
                "Identity issuer key is missing",
                error.Message);

        if (error?.Code == IdentityErrorCodes.InvalidIssuerKey)
            return ApiProblemDetails.Build(
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                error.Message);

        if (error?.Code == IdentityErrorCodes.InvalidRole)
        {
            return ApiProblemDetails.Build(
                StatusCodes.Status400BadRequest,
                "Invalid role",
                error.Message);
        }

        if (error?.Code == IdentityErrorCodes.InvalidScope)
        {
            return ApiProblemDetails.Build(
                StatusCodes.Status400BadRequest,
                "Invalid scope",
                error.Message);
        }

        return ApiProblemDetails.Build(
            StatusCodes.Status400BadRequest,
            "Token generation failed",
            error?.Message ?? "Unexpected error.");
    }
}

public record TokenRequest(string UserId, string Role, string[]? Scopes = null);
