using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using AlertHub.Application.Common.Security;
using AlertHub.Application.Subscriptions;
using AlertHub.Domain.Alert;
using AlertHub.Domain.Subscriptions;
using AlertHub.Tests.Integration.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlertHub.Tests.Integration.Subscriptions;

[Collection("Postgres")]
public sealed class SubscriptionsControllerTests
{
    private readonly HttpClient _client;
    private readonly PostgresContainerFixture _fixture;

    public SubscriptionsControllerTests(PostgresContainerFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(Helpers.TestAuthHandler.AuthenticationScheme);
    }

    [Fact]
    public async Task CreateSubscription_WithValidRequest_ReturnsCreatedAndPersists()
    {
        // Arrange
        var request = new CreateSubscriptionRequest(
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
        var request = new CreateSubscriptionRequest(
            Channel: SubscriptionChannel.Sms,
            Target: "not-a-phone-number"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/subscriptions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSubscription_WithMissingUserIdClaim_ReturnsUnauthorized()
    {
        var factory = _fixture.Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(MissingUserIdAuthHandler.AuthenticationScheme)
                    .AddScheme<AuthenticationSchemeOptions, MissingUserIdAuthHandler>(
                        MissingUserIdAuthHandler.AuthenticationScheme, _ => { });
            });
        });

        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(MissingUserIdAuthHandler.AuthenticationScheme);

        var request = new CreateSubscriptionRequest(
            Channel: SubscriptionChannel.Email,
            Target: "test@example.com");

        var response = await client.PostAsJsonAsync("/api/subscriptions", request);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private sealed class MissingUserIdAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string AuthenticationScheme = "MissingUserIdScheme";

        public MissingUserIdAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder) : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.Role, Roles.Admin),
                new Claim(ClaimTypes.Role, Roles.Subscriber),
                new Claim("scope", Scopes.AlertsIngest)
            };

            var identity = new ClaimsIdentity(claims, AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, AuthenticationScheme);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
