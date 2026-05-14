using Testcontainers.MsSql;

namespace Luga.Tests.Integration.Fixtures;

/// <summary>
/// Boots a throwaway SQL Server container shared by every test in the collection.
/// Tests that need an isolated database create their own with a unique name on
/// the same container.
/// </summary>
public sealed class SqlServerFixture : IAsyncLifetime
{
    public MsSqlContainer Container { get; } = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Pa55w0rd!StrongDev")
        .Build();

    public string ConnectionString => Container.GetConnectionString();

    public Task InitializeAsync() => Container.StartAsync();

    public Task DisposeAsync() => Container.DisposeAsync().AsTask();
}

/// <summary>
/// xUnit collection definition so multiple test classes share the same container.
/// </summary>
[CollectionDefinition(Name)]
public sealed class SqlServerCollection : ICollectionFixture<SqlServerFixture>
{
    public const string Name = "SqlServer";
}
