namespace AlertHub.Infrastructure.Persistence.Entities;

public sealed class AlertAreaGeoCodeEntity
{
    public Guid Id { get; set; }

    public Guid AlertAreaId { get; set; }

    public string ValueName { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public AlertAreaEntity Area { get; set; } = null!;
}
