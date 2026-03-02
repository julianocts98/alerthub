namespace AlertHub.Infrastructure.Persistence.Entities;

public sealed class AlertAreaEntity
{
    public Guid Id { get; set; }

    public Guid AlertInfoId { get; set; }

    public string AreaDescription { get; set; } = string.Empty;

    public double? Altitude { get; set; }

    public double? Ceiling { get; set; }

    public AlertInfoEntity Info { get; set; } = null!;

    public List<AlertAreaPolygonEntity> Polygons { get; set; } = [];

    public List<AlertAreaCircleEntity> Circles { get; set; } = [];

    public List<AlertAreaGeoCodeEntity> GeoCodes { get; set; } = [];
}
