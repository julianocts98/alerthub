using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AlertHub.Application.Common;
using AlertHub.Application.Common.Security;
using Microsoft.IdentityModel.Tokens;

namespace AlertHub.Application.Identity;

public static class IdentityErrorCodes
{
    public const string IssuerKeyNotConfigured = "identity.issuer_key.not_found";
    public const string InvalidIssuerKey = "identity.issuer_key.unauthorized";
    public const string InvalidRole = "identity.role.invalid";
    public const string InvalidScope = "identity.scope.invalid";
}

public sealed record IssueTokenCommand(string UserId, string Role, string[]? Scopes = null);

public sealed record IssuedToken(string Value);

public sealed class IdentityService
{
    private readonly IConfiguration _configuration;

    public IdentityService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Result<IssuedToken> IssueToken(IssueTokenCommand command, string? providedIssuerKey)
    {
        var configuredDemoKey = _configuration["Identity:IssuerApiKey"];
        if (string.IsNullOrWhiteSpace(configuredDemoKey))
        {
            return Result<IssuedToken>.Failure(
                new ResultError(IdentityErrorCodes.IssuerKeyNotConfigured, "Identity issuer key is not configured."));
        }

        if (!string.Equals(configuredDemoKey, providedIssuerKey, StringComparison.Ordinal))
        {
            return Result<IssuedToken>.Failure(
                new ResultError(IdentityErrorCodes.InvalidIssuerKey, "Issuer key is invalid."));
        }

        if (!IsSupportedRole(command.Role))
        {
            return Result<IssuedToken>.Failure(
                new ResultError(IdentityErrorCodes.InvalidRole, "Role is not supported."));
        }

        var requestedScopes = command.Scopes ?? [];
        if (requestedScopes.Any(scope => !IsSupportedScope(scope)))
        {
            return Result<IssuedToken>.Failure(
                new ResultError(IdentityErrorCodes.InvalidScope, "One or more scopes are not supported."));
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, command.UserId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, command.Role)
        };

        foreach (var scope in requestedScopes)
            claims.Add(new Claim("scope", scope));

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

        return Result<IssuedToken>.Success(new IssuedToken(new JwtSecurityTokenHandler().WriteToken(token)));
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
