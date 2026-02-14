using AlertHub.Application.Common;

namespace AlertHub.Application.Alerts.Ingestion;

public interface ICapAlertParser
{
    bool CanHandle(string contentType);

    Result<AlertIngestionRequest> Parse(string rawPayload);
}
