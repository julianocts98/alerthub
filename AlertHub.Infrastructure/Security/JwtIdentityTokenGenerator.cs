using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AlertHub.Application.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AlertHub.Infrastructure.Security;

public sealed class JwtIdentityTokenGenerator : IIdentityTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtIdentityTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(
        string userId,
        string role,
        IReadOnlyCollection<string> scopes,
        DateTimeOffset expiresAtUtc)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, role)
        };

        foreach (var scope in scopes)
        {
            claims.Add(new Claim("scope", scope));
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
            expires: expiresAtUtc.UtcDateTime,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string RequireSetting(string key)
    {
        var value = _configuration[key];
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        throw new InvalidOperationException($"Missing required configuration value '{key}'.");
    }
}
