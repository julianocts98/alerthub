using System.Net;
using System.Net.Http.Json;
using System.Text;
using AlertHub.Application.Alerts.Ingestion;
using AlertHub.Api.Subscriptions;
using AlertHub.Domain.Subscriptions;
using AlertHub.Infrastructure.Persistence.Entities.Deliveries;
using AlertHub.Tests.Integration.Helpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using AlertHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AlertHub.Tests.Integration;

[Collection("Postgres")]
public sealed class EndToEndPipelineTests
{
    private readonly AlertsApiFactory _factory;
    private readonly HttpClient _client;

    public EndToEndPipelineTests(PostgresContainerFixture fixture)
    {
        _factory = fixture.Factory;
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme);
    }

    [Fact]
    public async Task IngestedAlert_ShouldEventuallyResultInDeliveryRecord()
    {
        // 1. Create a Subscription
        var subRequest = new CreateSubscriptionRequestDto(
            Channel: SubscriptionChannel.Telegram,
            Target: "12345678"
        );
        var subResponse = await _client.PostAsJsonAsync("/api/subscriptions", subRequest);
        subResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // 2. Ingest an Alert (XML is most reliable for CAP)
        var alertXml = """
        <?xml version="1.0" encoding="UTF-8"?>
        <alert xmlns="urn:oasis:names:tc:emergency:cap:1.2">
          <identifier>e2e-alert-001</identifier>
          <sender>e2e@test.com</sender>
          <sent>2026-02-16T10:00:00+00:00</sent>
          <status>Actual</status>
          <msgType>Alert</msgType>
          <scope>Public</scope>
          <info>
            <category>Met</category>
            <event>E2E Test</event>
            <urgency>Immediate</urgency>
            <severity>Extreme</severity>
            <certainty>Observed</certainty>
            <area>
              <areaDesc>Test Area</areaDesc>
            </area>
          </info>
        </alert>
        """;
        using var content = new StringContent(alertXml, Encoding.UTF8, "application/xml");
        var ingestResponse = await _client.PostAsync("/api/alerts/ingest", content);
        
        if (ingestResponse.StatusCode != HttpStatusCode.Created)
        {
            var error = await ingestResponse.Content.ReadAsStringAsync();
            throw new Exception($"Ingest failed with {ingestResponse.StatusCode}: {error}");
        }

        // 3. Wait for background processing (Polling)
        // Pipeline: Outbox -> RabbitMQ -> Matcher -> Delivery Table
        AlertDeliveryEntity? delivery = null;
        for (int i = 0; i < 10; i++)
        {
            await Task.Delay(2000); // Wait for background jobs to tick
            
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            delivery = await db.AlertDeliveries.FirstOrDefaultAsync(d => d.Target == "12345678");
            
            if (delivery != null) break;
        }

        // 4. Assert
        delivery.Should().NotBeNull("Background pipeline should have created a delivery record");
        delivery!.Status.Should().BeOneOf(DeliveryStatus.Pending, DeliveryStatus.Sent);
    }
}
