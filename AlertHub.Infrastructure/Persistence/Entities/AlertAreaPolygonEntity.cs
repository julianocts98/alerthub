namespace AlertHub.Infrastructure.Persistence.Entities;

public sealed class AlertAreaPolygonEntity
{
    public Guid Id { get; set; }

    public Guid AlertAreaId { get; set; }

    /// <summary>
    /// Polygon stored as the original CAP space-separated "lat,lon" string.
    /// Intended to be migrated to PostGIS geometry when spatial queries are needed.
    /// </summary>
    public string Points { get; set; } = string.Empty;

    public AlertAreaEntity Area { get; set; } = null!;
}
