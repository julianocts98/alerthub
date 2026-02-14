namespace AlertHub.Application.Alerts.Query;

public sealed class AlertQueryResult
{
    public Guid Id { get; init; }

    public string Identifier { get; init; } = string.Empty;

    public string Sender { get; init; } = string.Empty;

    public DateTimeOffset Sent { get; init; }

    public string Status { get; init; } = string.Empty;

    public string MessageType { get; init; } = string.Empty;

    public string Scope { get; init; } = string.Empty;

    public DateTimeOffset IngestedAtUtc { get; init; }

    public IReadOnlyList<AlertInfoQueryResult> Infos { get; init; } = [];
}

public sealed class AlertInfoQueryResult
{
    public Guid Id { get; init; }

    public string Event { get; init; } = string.Empty;

    public string Urgency { get; init; } = string.Empty;

    public string Severity { get; init; } = string.Empty;

    public string Certainty { get; init; } = string.Empty;

    public IReadOnlyList<string> Categories { get; init; } = [];

    public DateTimeOffset? Effective { get; init; }

    public DateTimeOffset? Onset { get; init; }

    public DateTimeOffset? Expires { get; init; }

    public string? Headline { get; init; }
}
