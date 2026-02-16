using AlertHub.Domain.Alert;
using AlertHub.Domain.Common;

namespace AlertHub.Infrastructure.Persistence.Entities;

public sealed class AlertEntity : AggregateRoot
{
    public Guid Id { get; set; }

    public string Identifier { get; set; } = string.Empty;

    public string Sender { get; set; } = string.Empty;

    public DateTimeOffset Sent { get; set; }

    public AlertStatus Status { get; set; }

    public AlertMessageType MessageType { get; set; }

    public AlertScope Scope { get; set; }

    public string? Source { get; set; }

    public string? Restriction { get; set; }

    public string? Note { get; set; }

    /// <summary>Space-separated list as per CAP spec.</summary>
    public string? Addresses { get; set; }

    /// <summary>Space-separated list as per CAP spec.</summary>
    public string? Codes { get; set; }

    /// <summary>Space-separated list of "sender,identifier,sent" triples as per CAP spec.</summary>
    public string? References { get; set; }

    /// <summary>Space-separated list as per CAP spec.</summary>
    public string? Incidents { get; set; }

    public string RawPayload { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public DateTimeOffset IngestedAtUtc { get; set; }

    public List<AlertInfoEntity> Infos { get; set; } = [];
}
