using AlertHub.Application.Common;

namespace AlertHub.Application.Alerts.Query;

public sealed class AlertPage : PagedResponse<AlertQueryResult>
{
    public AlertPage(IReadOnlyList<AlertQueryResult> items, string? nextCursor)
    {
        Items = items;
        NextCursor = nextCursor;
    }
}
