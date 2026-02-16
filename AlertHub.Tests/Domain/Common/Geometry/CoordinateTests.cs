using AlertHub.Domain.Common;
using AlertHub.Domain.Common.Geometry;

namespace AlertHub.Tests.Domain.Common.Geometry;

public class CoordinateTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(90, 180)]
    [InlineData(-90, -180)]
    [InlineData(45.5, -122.6)]
    public void Constructor_WithValidCoordinates_ShouldCreateCoordinate(double lat, double lon)
    {
        var coord = new Coordinate(lat, lon);

        Assert.Equal(lat, coord.Latitude);
        Assert.Equal(lon, coord.Longitude);
    }

    [Theory]
    [InlineData(90.1, 0)]
    [InlineData(-90.1, 0)]
    [InlineData(0, 180.1)]
    [InlineData(0, -180.1)]
    public void Constructor_WithInvalidCoordinates_ShouldThrowDomainException(double lat, double lon)
    {
        var ex = Assert.Throws<DomainException>(() => new Coordinate(lat, lon));
        Assert.Contains("geometry.coordinate", ex.Error.Code);
    }

    [Theory]
    [InlineData("45.5,-122.6", 45.5, -122.6)]
    [InlineData("0,0", 0, 0)]
    [InlineData("-90,180", -90, 180)]
    public void Parse_WithValidString_ShouldReturnCoordinate(string input, double expectedLat, double expectedLon)
    {
        var result = Coordinate.Parse(input);

        Assert.Equal(expectedLat, result.Latitude);
        Assert.Equal(expectedLon, result.Longitude);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("45.5")]
    [InlineData("45.5,-122.6,0")]
    [InlineData("abc,def")]
    public void Parse_WithInvalidString_ShouldThrowDomainException(string input)
    {
        Assert.Throws<DomainException>(() => Coordinate.Parse(input));
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        var coord = new Coordinate(45.5, -122.6);

        var result = coord.ToString();

        Assert.Equal("45.5,-122.6", result);
    }
}
