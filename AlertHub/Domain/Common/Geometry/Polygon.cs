namespace AlertHub.Domain.Common.Geometry;

public sealed record Polygon
{
    public IReadOnlyList<Coordinate> Points { get; }

    public Polygon(IEnumerable<Coordinate> points)
    {
        var pointList = points.ToList();

        if (pointList.Count < 4)
            throw new DomainException(GeometryDomainErrors.PolygonInvalid);

        if (pointList[0] != pointList[^1])
            throw new DomainException(GeometryDomainErrors.PolygonInvalid);

        Points = pointList.AsReadOnly();
    }

    public static Polygon Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(GeometryDomainErrors.PolygonRequired);

        try
        {
            var coordinates = value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(Coordinate.Parse);

            return new Polygon(coordinates);
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new DomainException(GeometryDomainErrors.PolygonInvalid);
        }
    }

    public override string ToString() => string.Join(" ", Points.Select(p => p.ToString()));
}
