namespace AlertHub.Application.Identity;

public sealed record TokenRequest(string UserId, string Role, string[]? Scopes = null);
