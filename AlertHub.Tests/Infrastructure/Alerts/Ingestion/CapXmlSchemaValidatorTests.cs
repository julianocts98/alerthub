using AlertHub.Infrastructure.Alerts.Ingestion;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace AlertHub.Tests.Infrastructure.Alerts.Ingestion;

public sealed class CapXmlSchemaValidatorTests
{
    [Fact]
    public void Validate_XmlResourceWithoutMimeType_ShouldPass()
    {
        var environment = new TestHostEnvironment
        {
            ContentRootPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "AlertHub"))
        };

        var sut = new CapXmlSchemaValidator(environment);

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

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Development;
        public string ApplicationName { get; set; } = "AlertHub.Tests";
        public string ContentRootPath { get; set; } = string.Empty;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
