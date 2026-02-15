using AlertHub.Application.Alerts.Ingestion;
using AlertHub.Domain.Alert;

namespace AlertHub.Tests.Application.Alerts.Ingestion;

public class AlertFactoryTests
{
    private readonly AlertFactory _sut = new();

    [Fact]
    public async Task CreateAsync_WithValidRequest_ReturnsSuccess()
    {
        var request = BuildValidRequest();

        var result = await _sut.CreateAsync(request, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(request.Identifier, result.Value!.Identifier);
        Assert.Equal(request.Sent, result.Value.Sent);
    }

    [Fact]
    public async Task CreateAsync_WhenDomainRuleFails_ReturnsFailure()
    {
        var request = BuildValidRequest(
            scope: AlertScope.Private,
            addresses: []);

        var result = await _sut.CreateAsync(request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.NotNull(result.Error);
        Assert.Equal("alert.address.required_for_private", result.Error!.Code);
    }

    [Fact]
    public async Task CreateAsync_WhenIdentifierHasForbiddenChars_ReturnsFailure()
    {
        var request = BuildValidRequest(identifier: "bad id");

        var result = await _sut.CreateAsync(request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.NotNull(result.Error);
        Assert.Equal("alert.identifier.invalid_characters", result.Error!.Code);
    }

    private static AlertIngestionRequest BuildValidRequest(
        string identifier = "cap-alert-123",
        AlertScope scope = AlertScope.Public,
        List<string>? addresses = null)
    {
        addresses ??= ["target@example.com"];

        return new AlertIngestionRequest
        {
            Identifier = identifier,
            Sender = "alerts@example.com",
            Sent = DateTimeOffset.UtcNow,
            Status = AlertStatus.Actual,
            MessageType = AlertMessageType.Alert,
            Scope = scope,
            Addresses = string.Join(' ', addresses),
            Infos =
            [
                new AlertInfoRequest
                {
                    Event = "Severe weather warning",
                    Urgency = AlertUrgency.Immediate,
                    Severity = AlertSeverity.Severe,
                    Certainty = AlertCertainty.Observed,
                    Categories = [AlertInfoCategory.Met],
                    Areas =
                    [
                        new AlertAreaRequest
                        {
                            AreaDescription = "County A"
                        }
                    ]
                }
            ]
        };
    }
}
