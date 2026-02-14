namespace AlertHub.Application.Alerts.Query;

public sealed class AlertSearchQuery
{
    public string? Sender { get; init; }

    public string? Identifier { get; init; }

    public DateTimeOffset? SentFrom { get; init; }

    public DateTimeOffset? SentTo { get; init; }

    public string? Status { get; init; }

    public string? MessageType { get; init; }

    public string? Scope { get; init; }

    public string? Event { get; init; }

    public string? Urgency { get; init; }

    public string? Severity { get; init; }

    public string? Certainty { get; init; }

    public string? Category { get; init; }

    /// <summary>Opaque keyset cursor returned by the previous page. Null for the first page.</summary>
    public string? Cursor { get; init; }

    public int PageSize { get; init; } = 50;
}
