using AlertHub.Application.Alerts.Ingestion;
using AlertHub.Application.Common;

namespace AlertHub.Tests.Application.Alerts.Ingestion;

public class IngestAlertOrchestrationServiceTests
{
    private readonly IngestAlertService _service = new();

    [Fact]
    public async Task ExecuteAsync_WithUnsupportedContentType_ReturnsFailure()
    {
        var service = BuildService(new PassSchemaValidator());

        var result = await service.ExecuteAsync("{}", "text/plain", CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal(IngestionErrorCodes.UnsupportedContentType, result.Error!.Code);
    }

    [Fact]
    public async Task ExecuteAsync_WhenXmlSchemaInvalid_ReturnsFailure()
    {
        var service = BuildService(new FailSchemaValidator());

        const string xml = "<alert xmlns=\"urn:oasis:names:tc:emergency:cap:1.2\"></alert>";
        var result = await service.ExecuteAsync(xml, "application/xml", CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal(IngestionErrorCodes.XmlSchemaInvalid, result.Error!.Code);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidJson_ReturnsSuccess()
    {
        var service = BuildService(new PassSchemaValidator());

        const string json =
            """
            {
              "identifier": "cap-alert-123",
              "sender": "alerts@example.com",
              "sent": "2026-02-14T12:00:00+00:00",
              "status": "Actual",
              "messageType": "Alert",
              "scope": "Public",
              "info": [
                {
                  "category": ["Met"],
                  "event": "Severe weather warning",
                  "urgency": "Immediate",
                  "severity": "Severe",
                  "certainty": "Observed",
                  "area": [
                    {
                      "areaDesc": "County A"
                    }
                  ]
                }
              ]
            }
            """;

        var result = await service.ExecuteAsync(json, "application/json", CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("cap-alert-123", result.Value!.Identifier);
    }

    private IngestAlertOrchestrationService BuildService(ICapXmlSchemaValidator validator)
    {
        var parsers = new ICapAlertParser[]
        {
            new JsonCapAlertParser(),
            new XmlCapAlertParser()
        };

        return new IngestAlertOrchestrationService(parsers, validator, _service);
    }

    private sealed class PassSchemaValidator : ICapXmlSchemaValidator
    {
        public Result Validate(string rawXml) => Result.Success();
    }

    private sealed class FailSchemaValidator : ICapXmlSchemaValidator
    {
        public Result Validate(string rawXml) =>
            Result.Failure(new ResultError(IngestionErrorCodes.XmlSchemaInvalid, "Schema validation failed."));
    }
}
