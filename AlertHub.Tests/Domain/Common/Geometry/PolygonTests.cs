using AlertHub.Domain.Common;
using AlertHub.Domain.Common.Geometry;
using Xunit;

namespace AlertHub.Tests.Domain.Common.Geometry;

public class PolygonTests
{
    [Fact]
    public void Constructor_WithValidPoints_ShouldCreatePolygon()
    {
        var points = new[]
        {
            new Coordinate(0, 0),
            new Coordinate(0, 1),
            new Coordinate(1, 1),
            new Coordinate(0, 0)
        };

        var polygon = new Polygon(points);

        Assert.Equal(4, polygon.Points.Count);
        Assert.Equal(points, polygon.Points);
    }

    [Fact]
    public void Constructor_WithFewerThanFourPoints_ShouldThrowDomainException()
    {
        var points = new[]
        {
            new Coordinate(0, 0),
            new Coordinate(0, 1),
            new Coordinate(0, 0)
        };

        var ex = Assert.Throws<DomainException>(() => new Polygon(points));
        Assert.Equal("geometry.polygon.invalid", ex.Error.Code);
    }

    [Fact]
    public void Constructor_WhenNotClosed_ShouldThrowDomainException()
    {
        var points = new[]
        {
            new Coordinate(0, 0),
            new Coordinate(0, 1),
            new Coordinate(1, 1),
            new Coordinate(1, 0)
        };

        var ex = Assert.Throws<DomainException>(() => new Polygon(points));
        Assert.Equal("geometry.polygon.invalid", ex.Error.Code);
    }

    [Fact]
    public void Parse_WithValidString_ShouldReturnPolygon()
    {
        var input = "0,0 0,1 1,1 0,0";

        var result = Polygon.Parse(input);

        Assert.Equal(4, result.Points.Count);
        Assert.Equal(0, result.Points[0].Latitude);
        Assert.Equal(0, result.Points[3].Latitude);
    }

    [Theory]
    [InlineData("")]
    [InlineData("0,0 0,1 1,1")]
    [InlineData("0,0 0,1 1,1 1,0")]
    [InlineData("0,0 0,1 1,1 invalid")]
    public void Parse_WithInvalidString_ShouldThrowDomainException(string input)
    {
        Assert.Throws<DomainException>(() => Polygon.Parse(input));
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        var points = new[]
        {
            new Coordinate(0, 0),
            new Coordinate(0, 1),
            new Coordinate(1, 1),
            new Coordinate(0, 0)
        };
        var polygon = new Polygon(points);

        var result = polygon.ToString();

        Assert.Equal("0,0 0,1 1,1 0,0", result);
    }
}
