namespace AlertHub.Application.Common.Security;

public interface ICurrentUser
{
    string? Id { get; }
    bool IsAuthenticated { get; }
    bool HasRole(string role);
}

public static class Roles
{
    public const string Admin = "admin";
    public const string Subscriber = "subscriber";
}

public static class Scopes
{
    public const string AlertsIngest = "alerts:ingest";
}
