using System.ComponentModel.DataAnnotations;

namespace AlertHub.Application.Alerts.Ingestion;

public sealed class AlertIngestionResponse
{
    [Required]
    public Guid Id { get; init; }

    [Required]
    [MaxLength(200)]
    public string Identifier { get; init; } = string.Empty;

    [Required]
    public DateTimeOffset Sent { get; init; }

    [Required]
    public DateTimeOffset IngestedAtUtc { get; init; }
}
