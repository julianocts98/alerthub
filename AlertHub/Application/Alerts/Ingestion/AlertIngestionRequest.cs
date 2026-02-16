using AlertHub.Domain.Alert;

namespace AlertHub.Application.Alerts.Ingestion;

public sealed class AlertIngestionRequest
{
    public string Identifier { get; set; } = string.Empty;

    public string Sender { get; set; } = string.Empty;

    public DateTimeOffset Sent { get; set; }

    public AlertStatus Status { get; set; }

    public AlertMessageType MessageType { get; set; }

    public AlertScope Scope { get; set; }

    public string? Source { get; set; }

    public string? Restriction { get; set; }

    public string? Note { get; set; }

    public string? Addresses { get; set; }

    public List<string> Codes { get; set; } = [];

    public string? References { get; set; }

    public string? Incidents { get; set; }

    public List<AlertInfoRequest> Infos { get; set; } = [];
}

public sealed class AlertInfoRequest
{
    public string? Language { get; set; }

    public List<AlertInfoCategory> Categories { get; set; } = [];

    public string Event { get; set; } = string.Empty;

    public List<AlertResponseType> ResponseTypes { get; set; } = [];

    public AlertUrgency Urgency { get; set; }

    public AlertSeverity Severity { get; set; }

    public AlertCertainty Certainty { get; set; }

    public string? Audience { get; set; }

    public List<AlertKeyValueRequest> EventCodes { get; set; } = [];

    public DateTimeOffset? Effective { get; set; }

    public DateTimeOffset? Onset { get; set; }

    public DateTimeOffset? Expires { get; set; }

    public string? SenderName { get; set; }

    public string? Headline { get; set; }

    public string? Description { get; set; }

    public string? Instruction { get; set; }

    public string? Web { get; set; }

    public string? Contact { get; set; }

    public List<AlertKeyValueRequest> Parameters { get; set; } = [];

    public List<AlertResourceRequest> Resources { get; set; } = [];

    public List<AlertAreaRequest> Areas { get; set; } = [];
}

public sealed class AlertKeyValueRequest
{
    public string ValueName { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
}

public sealed class AlertResourceRequest
{
    public string ResourceDescription { get; set; } = string.Empty;

    public string MimeType { get; set; } = string.Empty;

    public long? Size { get; set; }

    public string? Uri { get; set; }

    public string? DerefUri { get; set; }

    public string? Digest { get; set; }
}

public sealed class AlertAreaRequest
{
    public string AreaDescription { get; set; } = string.Empty;

    public List<string> Polygons { get; set; } = [];

    public List<string> Circles { get; set; } = [];

    public List<AlertKeyValueRequest> GeoCodes { get; set; } = [];

    public double? Altitude { get; set; }

    public double? Ceiling { get; set; }
}
