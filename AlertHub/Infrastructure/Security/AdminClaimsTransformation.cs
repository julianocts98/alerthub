using System.Security.Claims;
using AlertHub.Application.Common.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;

namespace AlertHub.Infrastructure.Security;

public sealed class AdminClaimsTransformation : IClaimsTransformation
{
    private readonly List<string> _adminIds;

    public AdminClaimsTransformation(IConfiguration configuration)
    {
        _adminIds = configuration.GetSection("Security:AdminIds").Get<List<string>>() ?? new List<string>();
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId != null && _adminIds.Contains(userId))
        {
            // If the user is in the admin list, ensure they have the Admin role
            if (!principal.IsInRole(Roles.Admin))
            {
                var identity = principal.Identity as ClaimsIdentity;
                identity?.AddClaim(new Claim(ClaimTypes.Role, Roles.Admin));
            }
        }

        return Task.FromResult(principal);
    }
}
