using AlertHub.Infrastructure.Persistence;
using AlertHub.Tests.Integration.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlertHub.Tests.Integration;

public sealed class AlertsApiFactory : WebApplicationFactory<Program>
{
    private readonly PostgresContainerFixture _fixture;

    public AlertsApiFactory(PostgresContainerFixture fixture)
    {
        _fixture = fixture;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _fixture.ConnectionString,
                ["RabbitMQ:HostName"] = _fixture.RabbitHost,
                ["RabbitMQ:Port"] = _fixture.RabbitPort.ToString(),
                ["BackgroundJobs:OutboxIntervalMs"] = "100",
                ["BackgroundJobs:DeliveryIntervalMs"] = "100",
                ["Jwt:Issuer"] = "AlertHub",
                ["Jwt:Audience"] = "AlertHub",
                ["Jwt:Key"] = "a_very_long_secret_key_for_development_purposes"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.AuthenticationScheme, _ => { });
        });
    }

    public async Task InitializeDbAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
}
