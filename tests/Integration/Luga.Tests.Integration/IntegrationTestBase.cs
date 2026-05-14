using Luga.Tests.Integration.Fixtures;

namespace Luga.Tests.Integration;

/// <summary>
/// Shared boilerplate for integration tests: spins up the
/// <see cref="LugaWebApplicationFactory"/> against the shared SQL Server
/// container and exposes a configured <see cref="HttpClient"/>.
/// </summary>
[Collection(SqlServerCollection.Name)]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    private readonly SqlServerFixture _sqlServer;

    protected IntegrationTestBase(SqlServerFixture sqlServer)
    {
        _sqlServer = sqlServer;
    }

    protected LugaWebApplicationFactory Factory { get; private set; } = default!;

    protected HttpClient Client { get; private set; } = default!;

    public Task InitializeAsync()
    {
        Factory = new LugaWebApplicationFactory(_sqlServer.ConnectionString);
        Client = Factory.CreateClient();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        Factory.Dispose();
        return Task.CompletedTask;
    }
}
