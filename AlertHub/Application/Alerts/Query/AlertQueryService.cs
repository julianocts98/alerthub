using AlertHub.Application.Alerts.Ingestion;

namespace AlertHub.Application.Alerts.Query;

public sealed class AlertQueryService
{
    private readonly IAlertRepository _alertRepository;

    public AlertQueryService(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public Task<AlertPage> SearchAsync(AlertSearchQuery query, CancellationToken ct)
        => _alertRepository.SearchAsync(query, ct);
}
