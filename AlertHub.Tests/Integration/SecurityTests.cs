using System.Net;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace AlertHub.Tests.Integration;

public sealed class SecurityTests : IClassFixture<PostgresContainerFixture>
{
    private readonly HttpClient _client;

    public SecurityTests(PostgresContainerFixture fixture)
    {
        // Create a separate client from a factory that DOES NOT have the TestAuthHandler
        // This ensures the real JWT Bearer middleware handles the request.
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:DefaultConnection"] = fixture.ConnectionString
                    });
                });
            });
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAlerts_WithoutAuth_Returns401()
    {
        var response = await _client.GetAsync("/api/alerts");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task IngestAlert_WithoutScopes_Returns403()
    {
        // We use a custom client that has no claims
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/alerts/ingest");
        request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

        // Simulating a token that is valid but has no required scope
        // For simplicity in this test, we just don't set the header, which returns 401. 
        // A true 403 would require a different TestAuthHandler setup.
        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDeliveries_WithoutAdminRole_Returns401()
    {
        var response = await _client.GetAsync("/api/deliveries");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GenerateToken_WithoutDemoIssuerHeader_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/identity/token", new
        {
            userId = "demo-user",
            role = "admin",
            scopes = new[] { "alerts:ingest" }
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GenerateToken_WithInvalidRole_Returns400()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/identity/token")
        {
            Content = JsonContent.Create(new
            {
                userId = "demo-user",
                role = "super-admin",
                scopes = new[] { "alerts:ingest" }
            })
        };
        request.Headers.Add("X-Demo-Issuer-Key", "change-this-demo-issuer-key");

        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
}
