using AlertHub.Domain.Common;
using AlertHub.Domain.Common.Geometry;
using Xunit;

namespace AlertHub.Tests.Domain.Common.Geometry;

public class CircleTests
{
    [Fact]
    public void Constructor_WithValidValues_ShouldCreateCircle()
    {
        var center = new Coordinate(45.5, -122.6);
        var radius = 10.5;

        var circle = new Circle(center, radius);

        Assert.Equal(center, circle.Center);
        Assert.Equal(radius, circle.Radius);
    }

    [Fact]
    public void Constructor_WithNonPositiveRadius_ShouldThrowDomainException()
    {
        var center = new Coordinate(0, 0);

        var ex = Assert.Throws<DomainException>(() => new Circle(center, -1));
        Assert.Equal("geometry.circle.invalid", ex.Error.Code);

        ex = Assert.Throws<DomainException>(() => new Circle(center, 0));
        Assert.Equal("geometry.circle.invalid", ex.Error.Code);
    }

    [Theory]
    [InlineData("45.5,-122.6 10.5", 45.5, -122.6, 10.5)]
    public void Parse_WithValidString_ShouldReturnCircle(string input, double lat, double lon, double radius)
    {
        var result = Circle.Parse(input);

        Assert.Equal(lat, result.Center.Latitude);
        Assert.Equal(lon, result.Center.Longitude);
        Assert.Equal(radius, result.Radius);
    }

    [Theory]
    [InlineData("")]
    [InlineData("45.5,-122.6")]
    [InlineData("45.5,-122.6 10.5 5")]
    [InlineData("invalid 10.5")]
    [InlineData("0,0 0")]
    [InlineData("0,0 abc")]
    public void Parse_WithInvalidString_ShouldThrowDomainException(string input)
    {
        Assert.Throws<DomainException>(() => Circle.Parse(input));
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        var circle = new Circle(new Coordinate(45.5, -122.6), 10.5);

        var result = circle.ToString();

        Assert.Equal("45.5,-122.6 10.5", result);
    }
}
