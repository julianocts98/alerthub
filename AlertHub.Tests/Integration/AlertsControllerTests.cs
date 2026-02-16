using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AlertHub.Application.Alerts.Ingestion;
using AlertHub.Application.Alerts.Query;
using AlertHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AlertHub.Tests.Integration;

[Collection("Postgres")]
public sealed class AlertsControllerTests : IAsyncLifetime
{
    private readonly AlertsApiFactory _factory;
    private HttpClient _client = default!;

    private const string ValidJsonAlert =
        """
        {
          "identifier": "test-alert-001",
          "sender": "test@example.com",
          "sent": "2026-02-15T10:00:00+00:00",
          "status": "Actual",
          "messageType": "Alert",
          "scope": "Public",
          "info": [
            {
              "category": ["Met"],
              "event": "Test weather event",
              "urgency": "Immediate",
              "severity": "Severe",
              "certainty": "Observed",
              "area": [
                {
                  "areaDesc": "Test Area"
                }
              ]
            }
          ]
        }
        """;

    // Valid CAP 1.2 XML that passes schema validation but fails domain publication
    // rule: scope=Restricted requires a restriction element.
    private const string RestrictedScopeXmlNoRestriction =
        """
        <?xml version="1.0" encoding="UTF-8"?>
        <alert xmlns="urn:oasis:names:tc:emergency:cap:1.2">
          <identifier>test-xml-restricted-001</identifier>
          <sender>test@example.com</sender>
          <sent>2026-02-15T10:00:00+00:00</sent>
          <status>Actual</status>
          <msgType>Alert</msgType>
          <scope>Restricted</scope>
          <info>
            <category>Met</category>
            <event>Test weather event</event>
            <urgency>Immediate</urgency>
            <severity>Severe</severity>
            <certainty>Observed</certainty>
            <area>
              <areaDesc>Test Area</areaDesc>
            </area>
          </info>
        </alert>
        """;

    public AlertsControllerTests(PostgresContainerFixture fixture)
    {
        _factory = fixture.Factory;
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(Helpers.TestAuthHandler.AuthenticationScheme);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE alerts RESTART IDENTITY CASCADE;");
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task PostIngest_WithValidJson_Returns201AndIdentifier()
    {
        using var content = new StringContent(ValidJsonAlert, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/alerts/ingest", content);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<AlertIngestionResponse>();
        Assert.NotNull(body);
        Assert.Equal("test-alert-001", body!.Identifier);
    }

    [Fact]
    public async Task PostIngest_WithUnsupportedContentType_Returns415()
    {
        using var content = new StringContent(ValidJsonAlert, Encoding.UTF8, "text/plain");

        var response = await _client.PostAsync("/api/alerts/ingest", content);

        Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
    }

    [Fact]
    public async Task PostIngest_WithXmlFailingDomainValidation_Returns422()
    {
        using var content = new StringContent(
            RestrictedScopeXmlNoRestriction, Encoding.UTF8, "application/xml");

        var response = await _client.PostAsync("/api/alerts/ingest", content);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task GetAlerts_WithNoData_Returns200AndEmptyItems()
    {
        var response = await _client.GetAsync("/api/alerts");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<AlertPage>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(body);
        Assert.Empty(body!.Items);
    }

    [Fact]
    public async Task GetAlerts_AfterIngest_Returns200AndAlertPresent()
    {
        using var ingestContent = new StringContent(ValidJsonAlert, Encoding.UTF8, "application/json");
        var ingestResponse = await _client.PostAsync("/api/alerts/ingest", ingestContent);
        Assert.Equal(HttpStatusCode.Created, ingestResponse.StatusCode);

        var response = await _client.GetAsync("/api/alerts");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<AlertPage>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(body);
        Assert.Single(body!.Items);
        Assert.Equal("test-alert-001", body.Items[0].Identifier);
    }

    [Fact]
    public async Task PostIngest_DuplicateAlert_Returns200AndWasAlreadyIngestedTrue()
    {
        // First ingestion
        using var content1 = new StringContent(ValidJsonAlert, Encoding.UTF8, "application/json");
        var response1 = await _client.PostAsync("/api/alerts/ingest", content1);
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);

        // Second ingestion (duplicate)
        using var content2 = new StringContent(ValidJsonAlert, Encoding.UTF8, "application/json");
        var response2 = await _client.PostAsync("/api/alerts/ingest", content2);

        // Assert idempotent success
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        var body = await response2.Content.ReadFromJsonAsync<AlertIngestionResponse>();
        Assert.NotNull(body);
        Assert.True(body!.WasAlreadyIngested);
    }
}
