namespace AlertHub.Application.Common.Security;

public interface ICurrentUser
{
    string? Id { get; }
    bool IsAuthenticated { get; }
    bool HasRole(string role);
}

public static class Roles
{
    public const string Producer = "Alert.Producer";
    public const string Consumer = "Alert.Consumer";
    public const string Admin = "Alert.Admin";
}
