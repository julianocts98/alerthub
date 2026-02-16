using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AlertHub.Application.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AlertHub.Api.Identity;

[ApiController]
[Route("api/[controller]")]
public sealed class IdentityController : ControllerBase
{
    private const string DemoIssuerHeaderName = "X-Demo-Issuer-Key";
    private readonly IConfiguration _configuration;

    public IdentityController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("token")]
    public IActionResult GenerateToken([FromBody] TokenRequest request)
    {
        var configuredDemoKey = _configuration["Identity:IssuerApiKey"];
        if (string.IsNullOrWhiteSpace(configuredDemoKey))
            return NotFound();

        var providedDemoKey = Request.Headers[DemoIssuerHeaderName].FirstOrDefault();
        if (!string.Equals(configuredDemoKey, providedDemoKey, StringComparison.Ordinal))
            return Unauthorized();

        if (!IsSupportedRole(request.Role))
            return BadRequest(new ProblemDetails { Title = "Invalid role", Detail = "Role is not supported.", Status = StatusCodes.Status400BadRequest });

        var requestedScopes = request.Scopes ?? [];
        if (requestedScopes.Any(scope => !IsSupportedScope(scope)))
            return BadRequest(new ProblemDetails { Title = "Invalid scope", Detail = "One or more scopes are not supported.", Status = StatusCodes.Status400BadRequest });

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, request.UserId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, request.Role)
        };

        if (request.Scopes != null)
        {
            foreach (var scope in request.Scopes)
            {
                claims.Add(new Claim("scope", scope));
            }
        }

        var jwtIssuer = RequireSetting("Jwt:Issuer");
        var jwtAudience = RequireSetting("Jwt:Audience");
        var jwtKey = RequireSetting("Jwt:Key");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }

    private string RequireSetting(string key)
    {
        var value = _configuration[key];
        if (!string.IsNullOrWhiteSpace(value))
            return value;

        throw new InvalidOperationException($"Missing required configuration value '{key}'.");
    }

    private static bool IsSupportedRole(string role)
        => role is Roles.Admin or Roles.Subscriber;

    private static bool IsSupportedScope(string scope)
        => scope is Scopes.AlertsIngest;
}

public record TokenRequest(string UserId, string Role, string[]? Scopes = null);
