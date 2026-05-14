// Luga.AppHost — orquestração Aspire para dev local (CLAUDE.md §19, ADR 040).
// Roda SQL Server em container + API + Blazor WASM.
// Dashboard: http://localhost:18888

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sql-server")
    .WithDataVolume("luga-sql-data")
    .WithLifetime(ContainerLifetime.Persistent);

var lugaDb = sqlServer.AddDatabase("luga-core");

var api = builder.AddProject<Projects.Luga_Server_Host>("api")
    .WithReference(lugaDb)
    .WaitFor(lugaDb);

builder.AddProject<Projects.Luga_Client_Host>("web")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
