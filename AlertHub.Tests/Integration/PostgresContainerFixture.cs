using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace AlertHub.Tests.Integration;

[CollectionDefinition("Postgres")]
public sealed class PostgresCollection : ICollectionFixture<PostgresContainerFixture> { }

public sealed class PostgresContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:16")
        .Build();

    private readonly RabbitMqContainer _rabbitContainer = new RabbitMqBuilder("rabbitmq:4.0-management")
        .WithUsername("alerthub")
        .WithPassword("alerthub")
        .Build();

    public AlertsApiFactory Factory { get; private set; } = default!;

    public string ConnectionString => _dbContainer.GetConnectionString();
    public string RabbitHost => _rabbitContainer.Hostname;
    public int RabbitPort => _rabbitContainer.GetMappedPublicPort(5672);

    public async Task InitializeAsync()
    {
        await Task.WhenAll(_dbContainer.StartAsync(), _rabbitContainer.StartAsync());
        Factory = new AlertsApiFactory(this);
        await Factory.InitializeDbAsync();
    }

    public async Task DisposeAsync()
    {
        if (Factory != null) await Factory.DisposeAsync();
        await Task.WhenAll(_dbContainer.DisposeAsync().AsTask(), _rabbitContainer.DisposeAsync().AsTask());
    }
}
