using AlertHub.Application.Identity;

namespace AlertHub.Infrastructure.Security;

public sealed class ConfigurationIdentityIssuerKeyProvider : IIdentityIssuerKeyProvider
{
    private readonly IConfiguration _configuration;

    public ConfigurationIdentityIssuerKeyProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string? GetIssuerKey()
    {
        return _configuration["Identity:IssuerApiKey"];
    }
}
