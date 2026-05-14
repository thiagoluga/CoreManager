// Luga.AppHost — Aspire orchestrator for local dev (CLAUDE.md §19, ADR 040).
// Spins up SQL Server in a container, then the API and Blazor WASM hosts wired
// to it. Dashboard at http://localhost:18888 (configured by Aspire defaults).

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

// SQL Server container with a persistent volume so dev data survives restarts.
var sqlServer = builder.AddSqlServer("sql-server")
    .WithDataVolume("luga-sql-data")
    .WithLifetime(ContainerLifetime.Persistent);

var lugaDb = sqlServer.AddDatabase("luga-core");

// API host. The reference injects ConnectionStrings__luga-core; we additionally
// publish it under the keys the app reads (Default + Hangfire) so the same code
// works under Aspire AND standalone with appsettings.json overrides.
var api = builder.AddProject<Projects.Luga_Server_Host>("api")
    .WithReference(lugaDb)
    .WithEnvironment("ConnectionStrings__Default", lugaDb.Resource.ConnectionStringExpression)
    .WithEnvironment("ConnectionStrings__Hangfire", lugaDb.Resource.ConnectionStringExpression)
    .WaitFor(lugaDb);

// Blazor WASM host. WithReference exposes the API base address so HttpClient
// configurations can pick it up.
builder.AddProject<Projects.Luga_Client_Host>("web")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
