using System.Net;
using AlertHub.Tests.Integration.Helpers;
using FluentAssertions;

namespace AlertHub.Tests.Integration;

[Collection("Postgres")]
public sealed class DeliveriesControllerTests
{
    private readonly HttpClient _client;

    public DeliveriesControllerTests(PostgresContainerFixture fixture)
    {
        _client = fixture.Factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme);
    }

    [Fact]
    public async Task GetDeliveries_Returns200()
    {
        var response = await _client.GetAsync("/api/deliveries");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RetryDelivery_WithNonExistentId_Returns404()
    {
        var response = await _client.PostAsync($"/api/deliveries/{Guid.NewGuid()}/retry", null);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
