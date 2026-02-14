using System.ComponentModel.DataAnnotations;
using AlertHub.Domain.Alert;

namespace AlertHub.Application.Alerts.Ingestion;

public sealed class AlertIngestionRequest
{
    [Required]
    [MaxLength(200)]
    [RegularExpression(@"^[^ ,<&]+$")]
    public string Identifier { get; init; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [RegularExpression(@"^[^ ,<&]+$")]
    public string Sender { get; init; } = string.Empty;

    [Required]
    public DateTimeOffset Sent { get; init; }

    [Required]
    [EnumDataType(typeof(AlertStatus))]
    public AlertStatus Status { get; init; }

    [Required]
    [EnumDataType(typeof(AlertMessageType))]
    public AlertMessageType MessageType { get; init; }

    [Required]
    [EnumDataType(typeof(AlertScope))]
    public AlertScope Scope { get; init; }

    [MaxLength(500)]
    public string? Source { get; init; }

    [MaxLength(1000)]
    public string? Restriction { get; init; }

    [MaxLength(4000)]
    public string? Note { get; init; }

    public List<string> Addresses { get; init; } = [];

    public List<string> Codes { get; init; } = [];

    public List<string> References { get; init; } = [];

    public List<string> Incidents { get; init; } = [];

    public List<AlertInfoRequest> Infos { get; init; } = [];
}

public sealed class AlertInfoRequest
{
    [Required]
    [MaxLength(300)]
    public string Event { get; init; } = string.Empty;

    [Required]
    [EnumDataType(typeof(AlertUrgency))]
    public AlertUrgency Urgency { get; init; }

    [Required]
    [EnumDataType(typeof(AlertSeverity))]
    public AlertSeverity Severity { get; init; }

    [Required]
    [EnumDataType(typeof(AlertCertainty))]
    public AlertCertainty Certainty { get; init; }

    [MinLength(1)]
    public List<AlertInfoCategory> Categories { get; init; } = [];

    public List<string> AreaDescriptions { get; init; } = [];

    [MaxLength(20)]
    public string? Language { get; init; }

    [MaxLength(200)]
    public string? Audience { get; init; }

    public DateTimeOffset? Effective { get; init; }

    public DateTimeOffset? Onset { get; init; }

    public DateTimeOffset? Expires { get; init; }

    [MaxLength(200)]
    public string? SenderName { get; init; }

    [MaxLength(300)]
    public string? Headline { get; init; }

    [MaxLength(4000)]
    public string? Description { get; init; }

    [MaxLength(4000)]
    public string? Instruction { get; init; }

    [MaxLength(500)]
    public string? Web { get; init; }

    [MaxLength(300)]
    public string? Contact { get; init; }
}
