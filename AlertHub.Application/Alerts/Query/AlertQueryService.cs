namespace AlertHub.Application.Alerts.Query;

public sealed class AlertQueryService : IAlertQueryService
{
    private readonly IAlertRepository _alertRepository;

    public AlertQueryService(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public Task<AlertPage> SearchAsync(AlertSearchQuery query, CancellationToken ct)
        => _alertRepository.SearchAsync(query, ct);
}
