namespace AlertHub.Application.Alerts.Query;

public interface IAlertQueryService
{
    Task<AlertPage> SearchAsync(AlertSearchQuery query, CancellationToken ct);
}
