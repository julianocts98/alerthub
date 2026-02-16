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
        return _identityService.IssueToken(
            new IssueTokenCommand(request.UserId, request.Role, request.Scopes),
            providedDemoKey).ToActionResult(token => Ok(new { token = token!.Value }));
    }
}

public record TokenRequest(string UserId, string Role, string[]? Scopes = null);
