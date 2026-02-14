using AlertHub.Domain.Alert;

namespace AlertHub.Infrastructure.Persistence.Entities;

public sealed class AlertInfoEntity
{
    public Guid Id { get; set; }

    public Guid AlertId { get; set; }

    public string Event { get; set; } = string.Empty;

    public AlertUrgency Urgency { get; set; }

    public AlertSeverity Severity { get; set; }

    public AlertCertainty Certainty { get; set; }

    public string? Language { get; set; }

    public string? Audience { get; set; }

    public DateTimeOffset? Effective { get; set; }

    public DateTimeOffset? Onset { get; set; }

    public DateTimeOffset? Expires { get; set; }

    public string? SenderName { get; set; }

    public string? Headline { get; set; }

    public string? Description { get; set; }

    public string? Instruction { get; set; }

    public string? Web { get; set; }

    public string? Contact { get; set; }

    public AlertEntity Alert { get; set; } = null!;

    public List<AlertInfoCategoryEntity> Categories { get; set; } = [];

    public List<AlertInfoResponseTypeEntity> ResponseTypes { get; set; } = [];

    public List<AlertInfoEventCodeEntity> EventCodes { get; set; } = [];

    public List<AlertInfoParameterEntity> Parameters { get; set; } = [];

    public List<AlertInfoResourceEntity> Resources { get; set; } = [];

    public List<AlertAreaEntity> Areas { get; set; } = [];
}
