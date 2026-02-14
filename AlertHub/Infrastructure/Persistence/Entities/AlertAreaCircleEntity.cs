namespace AlertHub.Infrastructure.Persistence.Entities;

public sealed class AlertAreaCircleEntity
{
    public Guid Id { get; set; }

    public Guid AlertAreaId { get; set; }

    public double CenterLatitude { get; set; }

    public double CenterLongitude { get; set; }

    /// <summary>Radius in kilometres as per the CAP spec.</summary>
    public double Radius { get; set; }

    public AlertAreaEntity Area { get; set; } = null!;
}
