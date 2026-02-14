namespace AlertHub.Infrastructure.Persistence.Entities;

public sealed class AlertInfoEventCodeEntity
{
    public Guid Id { get; set; }

    public Guid AlertInfoId { get; set; }

    public string ValueName { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public AlertInfoEntity Info { get; set; } = null!;
}
