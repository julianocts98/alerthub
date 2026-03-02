using AlertHub.Infrastructure.Alerts.Ingestion;

namespace AlertHub.Tests.Infrastructure.Alerts.Ingestion;

public sealed class CapXmlSchemaValidatorTests
{
    [Fact]
    public void Validate_XmlResourceWithoutMimeType_ShouldPass()
    {
        var sut = new CapXmlSchemaValidator();

        const string xml =
            """
            <?xml version="1.0" encoding="UTF-8"?>
            <alert xmlns="urn:oasis:names:tc:emergency:cap:1.2">
              <identifier>mime-optional-001</identifier>
              <sender>alerts@example.com</sender>
              <sent>2026-02-16T10:00:00+00:00</sent>
              <status>Actual</status>
              <msgType>Alert</msgType>
              <scope>Public</scope>
              <info>
                <category>Met</category>
                <event>Resource without mimeType</event>
                <urgency>Immediate</urgency>
                <severity>Severe</severity>
                <certainty>Observed</certainty>
                <resource>
                  <resourceDesc>Attachment without mimeType</resourceDesc>
                </resource>
              </info>
            </alert>
            """;

        var result = sut.Validate(xml);

        Assert.True(result.IsSuccess);
    }
}
