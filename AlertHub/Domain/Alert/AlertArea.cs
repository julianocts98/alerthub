using AlertHub.Domain.Common;
using AlertHub.Domain.Common.Geometry;

namespace AlertHub.Domain.Alert;

public class AlertArea
{
    public Guid Id { get; } = Guid.NewGuid();

    public string AreaDescription { get; }

    public IReadOnlyCollection<Polygon> Polygons => _polygons;

    public IReadOnlyCollection<Circle> Circles => _circles;

    public IReadOnlyCollection<AlertGeoCode> GeoCodes => _geoCodes;

    public double? Altitude { get; private set; }

    public double? Ceiling { get; private set; }

    private readonly List<Polygon> _polygons = [];
    private readonly List<Circle> _circles = [];
    private readonly List<AlertGeoCode> _geoCodes = [];

    internal AlertArea(string areaDescription)
    {
        if (string.IsNullOrWhiteSpace(areaDescription))
            throw new DomainException(AlertDomainErrors.AreaDescriptionRequired);

        AreaDescription = areaDescription;
    }

    internal void AddPolygon(Polygon polygon)
    {
        ArgumentNullException.ThrowIfNull(polygon);
        _polygons.Add(polygon);
    }

    internal void AddCircle(Circle circle)
    {
        ArgumentNullException.ThrowIfNull(circle);
        _circles.Add(circle);
    }

    void AddGeoCode(string valueName, string value)
    {
        _geoCodes.Add(new AlertGeoCode(valueName, value));
    }

    internal void SetAltitude(double? altitude)
    {
        Altitude = altitude;
    }

    internal void SetCeiling(double? ceiling)
    {
        Ceiling = ceiling;
    }
}
