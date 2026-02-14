using AlertHub.Domain.Alert;

namespace AlertHub.Infrastructure.Persistence.Entities;

public sealed class IngestedAlertEntity
{
    public Guid Id { get; set; }

    public string Identifier { get; set; } = string.Empty;

    public string Sender { get; set; } = string.Empty;

    public DateTimeOffset Sent { get; set; }

    public AlertStatus Status { get; set; }

    public AlertMessageType MessageType { get; set; }

    public AlertScope Scope { get; set; }

    public string RawPayload { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public DateTimeOffset IngestedAtUtc { get; set; }
}
