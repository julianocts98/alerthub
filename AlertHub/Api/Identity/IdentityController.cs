using AlertHub.Application.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AlertHub.Api.Identity;

[ApiController]
[Route("api/[controller]")]
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
            return NotFound();

        if (error?.Code == IdentityErrorCodes.InvalidIssuerKey)
            return Unauthorized();

        if (error?.Code == IdentityErrorCodes.InvalidRole)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid role",
                Detail = error.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (error?.Code == IdentityErrorCodes.InvalidScope)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid scope",
                Detail = error.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return BadRequest(new ProblemDetails
        {
            Title = "Token generation failed",
            Detail = error?.Message ?? "Unexpected error.",
            Status = StatusCodes.Status400BadRequest
        });
    }
}

public record TokenRequest(string UserId, string Role, string[]? Scopes = null);
