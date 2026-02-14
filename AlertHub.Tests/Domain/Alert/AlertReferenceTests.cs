using AlertHub.Domain.Alert;
using AlertHub.Domain.Common;
using Xunit;

namespace AlertHub.Tests.Domain.Alert;

public class AlertReferenceTests
{
    [Fact]
    public void Constructor_WithValidValues_ShouldCreateReference()
    {
        var sender = "sender@example.com";
        var identifier = "12345";
        var sent = DateTimeOffset.Now;

        var reference = new AlertReference(sender, identifier, sent);

        Assert.Equal(sender, reference.Sender);
        Assert.Equal(identifier, reference.Identifier);
        Assert.Equal(sent, reference.Sent);
    }

    [Theory]
    [InlineData("", "12345")]
    [InlineData("sender@example.com", "")]
    public void Constructor_WithInvalidValues_ShouldThrowDomainException(string sender, string identifier)
    {
        Assert.Throws<DomainException>(() => new AlertReference(sender, identifier, DateTimeOffset.Now));
    }

    [Fact]
    public void Parse_WithValidString_ShouldReturnReference()
    {
        var input = "sender@example.com,12345,2026-02-14T12:00:00+00:00";

        var result = AlertReference.Parse(input);

        Assert.Equal("sender@example.com", result.Sender);
        Assert.Equal("12345", result.Identifier);
        Assert.Equal(2026, result.Sent.Year);
    }

    [Theory]
    [InlineData("")]
    [InlineData("sender,id")]
    [InlineData("sender,id,invalid-date")]
    public void Parse_WithInvalidString_ShouldThrowDomainException(string input)
    {
        Assert.Throws<DomainException>(() => AlertReference.Parse(input));
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        var sent = new DateTimeOffset(2026, 2, 14, 12, 0, 0, TimeSpan.Zero);
        var reference = new AlertReference("sender", "id", sent);

        var result = reference.ToString();

        Assert.Equal("sender,id,2026-02-14T12:00:00+00:00", result);
    }
}
