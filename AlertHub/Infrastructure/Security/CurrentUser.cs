using System.Security.Claims;
using AlertHub.Application.Common.Security;
using Microsoft.AspNetCore.Http;

namespace AlertHub.Infrastructure.Security;

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool HasRole(string role) => _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
}
