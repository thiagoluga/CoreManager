// Luga.AppHost — Aspire orchestrator for local dev (CLAUDE.md §19, ADR 040).
// Spins up SQL Server in a container, then the Server.Host which both serves
// the API and the Blazor WASM bundle on the same origin (CLAUDE.md §5.8 +
// §5.9). Dashboard at http://localhost:18888 (configured by Aspire defaults).

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

// SQL Server container with a persistent volume so dev data survives restarts.
var sqlServer = builder.AddSqlServer("sql-server")
    .WithDataVolume("luga-sql-data")
    .WithLifetime(ContainerLifetime.Persistent);

var lugaDb = sqlServer.AddDatabase("luga-core");

// Single host: Server.Host serves the API and the Blazor WASM bundle via
// UseBlazorFrameworkFiles + MapFallbackToFile, so WASM and API share an origin
// in dev (matching production). Connection string is republished under the
// keys the app reads (Default + Hangfire) so the same code works under Aspire
// AND standalone with appsettings.json overrides.
builder.AddProject<Projects.Luga_Server_Host>("api")
    .WithReference(lugaDb)
    .WithEnvironment("ConnectionStrings__Default", lugaDb.Resource.ConnectionStringExpression)
    .WithEnvironment("ConnectionStrings__Hangfire", lugaDb.Resource.ConnectionStringExpression)
    .WaitFor(lugaDb);

builder.Build().Run();
