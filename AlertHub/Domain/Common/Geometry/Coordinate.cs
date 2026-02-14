using System.Globalization;

namespace AlertHub.Domain.Common.Geometry;

public readonly record struct Coordinate
{
    public double Latitude { get; }
    public double Longitude { get; }

    public Coordinate(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90)
            throw new DomainException(new DomainError("geometry.coordinate.latitude.invalid", "Latitude must be between -90 and 90."));

        if (longitude is < -180 or > 180)
            throw new DomainException(new DomainError("geometry.coordinate.longitude.invalid", "Longitude must be between -180 and 180."));

        Latitude = latitude;
        Longitude = longitude;
    }

    public static Coordinate Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(new DomainError("geometry.coordinate.required", "Coordinate string is required."));

        var parts = value.Split(',');
        if (parts.Length != 2)
            throw new DomainException(new DomainError("geometry.coordinate.invalid_format", "Coordinate must be in 'latitude,longitude' format."));

        if (!double.TryParse(parts[0], CultureInfo.InvariantCulture, out var lat))
            throw new DomainException(new DomainError("geometry.coordinate.latitude.parse_error", "Could not parse latitude."));

        if (!double.TryParse(parts[1], CultureInfo.InvariantCulture, out var lon))
            throw new DomainException(new DomainError("geometry.coordinate.longitude.parse_error", "Could not parse longitude."));

        return new Coordinate(lat, lon);
    }

    public override string ToString() => $"{Latitude.ToString(CultureInfo.InvariantCulture)},{Longitude.ToString(CultureInfo.InvariantCulture)}";
}
