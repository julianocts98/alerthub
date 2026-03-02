using AlertHub.Domain.Common;
using AlertHub.Domain.Common.Security;

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

public sealed class IdentityService : IIdentityService
{
    private readonly IIdentityIssuerKeyProvider _issuerKeyProvider;
    private readonly IIdentityTokenGenerator _tokenGenerator;

    public IdentityService(IIdentityIssuerKeyProvider issuerKeyProvider, IIdentityTokenGenerator tokenGenerator)
    {
        _issuerKeyProvider = issuerKeyProvider;
        _tokenGenerator = tokenGenerator;
    }

    public Result<IssuedToken> IssueToken(IssueTokenCommand command, string? providedIssuerKey)
    {
        var configuredDemoKey = _issuerKeyProvider.GetIssuerKey();
        if (string.IsNullOrWhiteSpace(configuredDemoKey))
        {
            return Result<IssuedToken>.Failure(
                ResultError.NotFound(IdentityErrorCodes.IssuerKeyNotConfigured, "Identity issuer key is not configured."));
        }

        if (!string.Equals(configuredDemoKey, providedIssuerKey, StringComparison.Ordinal))
        {
            return Result<IssuedToken>.Failure(
                ResultError.Unauthorized(IdentityErrorCodes.InvalidIssuerKey, "Issuer key is invalid."));
        }

        if (!IsSupportedRole(command.Role))
        {
            return Result<IssuedToken>.Failure(
                ResultError.Validation(IdentityErrorCodes.InvalidRole, "Role is not supported."));
        }

        var requestedScopes = command.Scopes ?? [];
        if (requestedScopes.Any(scope => !IsSupportedScope(scope)))
        {
            return Result<IssuedToken>.Failure(
                ResultError.Validation(IdentityErrorCodes.InvalidScope, "One or more scopes are not supported."));
        }

        var token = _tokenGenerator.GenerateToken(
            command.UserId,
            command.Role,
            requestedScopes,
            DateTimeOffset.UtcNow.AddHours(2));

        return Result<IssuedToken>.Success(new IssuedToken(token));
    }

    private static bool IsSupportedRole(string role)
        => role is Roles.Admin or Roles.Subscriber;

    private static bool IsSupportedScope(string scope)
        => scope is Scopes.AlertsIngest;
}
