using System.Net;
using System.Net.Http.Json;
using AlertHub.Api.Subscriptions;
using AlertHub.Application.Subscriptions;
using AlertHub.Domain.Alert;
using AlertHub.Domain.Subscriptions;
using FluentAssertions;

namespace AlertHub.Tests.Integration.Subscriptions;

[Collection("Postgres")]
public sealed class SubscriptionsControllerTests
{
    private readonly HttpClient _client;

    public SubscriptionsControllerTests(PostgresContainerFixture fixture)
    {
        _client = fixture.Factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(Helpers.TestAuthHandler.AuthenticationScheme);
    }

    [Fact]
    public async Task CreateSubscription_WithValidRequest_ReturnsCreatedAndPersists()
    {
        // Arrange
        var request = new CreateSubscriptionRequestDto(
            Channel: SubscriptionChannel.Email,
            Target: "test@example.com",
            MinSeverity: AlertSeverity.Extreme,
            Categories: [AlertInfoCategory.Met, AlertInfoCategory.Safety]
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/subscriptions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<SubscriptionResponse>();
        result.Should().NotBeNull();
        result!.UserId.Should().Be("test-user-id"); // Injected by TestAuthHandler
        result.Channel.Should().Be(request.Channel);
        result.Target.Should().Be(request.Target);
        result.MinSeverity.Should().Be(request.MinSeverity);
        result.Categories.Should().BeEquivalentTo(request.Categories);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateSubscription_WithInvalidTarget_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateSubscriptionRequestDto(
            Channel: SubscriptionChannel.Sms,
            Target: "not-a-phone-number"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/subscriptions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
