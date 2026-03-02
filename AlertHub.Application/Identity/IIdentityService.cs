using AlertHub.Domain.Common;

namespace AlertHub.Application.Identity;

public interface IIdentityService
{
    Result<IssuedToken> IssueToken(IssueTokenCommand command, string? providedIssuerKey);
}
