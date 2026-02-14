namespace AlertHub.Domain.Common.Geometry;

public static class GeometryDomainErrors
{
    public static DomainError PolygonRequired =>
        new("geometry.polygon.required", "Polygon string is required.");

    public static DomainError PolygonInvalid =>
        new("geometry.polygon.invalid", "Polygon must have at least 4 coordinate pairs and be closed (first and last points must be identical).");

    public static DomainError CircleRequired =>
        new("geometry.circle.required", "Circle string is required.");

    public static DomainError CircleInvalid =>
        new("geometry.circle.invalid", "Circle must be a coordinate pair and a radius.");
}
