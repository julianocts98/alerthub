namespace AlertHub.Application.Identity;

public interface IIdentityTokenGenerator
{
    string GenerateToken(
        string userId,
        string role,
        IReadOnlyCollection<string> scopes,
        DateTimeOffset expiresAtUtc);
}
