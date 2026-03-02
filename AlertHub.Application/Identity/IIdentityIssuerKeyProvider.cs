namespace AlertHub.Application.Identity;

public interface IIdentityIssuerKeyProvider
{
    string? GetIssuerKey();
}
