using System.Globalization;

namespace AlertHub.Domain.Common.Geometry;

public sealed record Circle
{
    public Coordinate Center { get; }
    public double Radius { get; }

    public Circle(Coordinate center, double radius)
    {
        if (radius <= 0)
            throw new DomainException(GeometryDomainErrors.CircleInvalid);

        Center = center;
        Radius = radius;
    }

    public static Circle Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(GeometryDomainErrors.CircleRequired);

        var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
            throw new DomainException(GeometryDomainErrors.CircleInvalid);

        var center = Coordinate.Parse(parts[0]);

        if (!double.TryParse(parts[1], CultureInfo.InvariantCulture, out var radius))
            throw new DomainException(GeometryDomainErrors.CircleInvalid);

        return new Circle(center, radius);
    }

    public override string ToString() => $"{Center} {Radius.ToString(CultureInfo.InvariantCulture)}";
}
