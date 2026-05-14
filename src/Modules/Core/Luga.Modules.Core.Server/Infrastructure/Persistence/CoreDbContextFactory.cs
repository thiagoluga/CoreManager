using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Luga.Modules.Core.Server.Infrastructure.Persistence;

/// <summary>
/// Design-time factory consumed by <c>dotnet ef</c> tooling. Bypasses the host's
/// full DI graph so migrations can be scaffolded without resolving cross-module
/// abstractions (CLAUDE.md §7.9).
/// </summary>
/// <remarks>
/// Reads <c>ConnectionStrings:Default</c> from the env var
/// <c>ConnectionStrings__Default</c> when present; falls back to a LocalDB placeholder
/// that EF only needs for SQL provider selection (no real connection happens
/// during <c>migrations add</c> / <c>script</c>).
/// </remarks>
public sealed class CoreDbContextFactory : IDesignTimeDbContextFactory<CoreDbContext>
{
    public CoreDbContext CreateDbContext(string[] args)
    {
        string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=LugaCore;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;";

        DbContextOptionsBuilder<CoreDbContext> options = new();
        options.UseSqlServer(connectionString, sql =>
            sql.MigrationsHistoryTable("__EFMigrationsHistory", schema: "core"));

        return new CoreDbContext(options.Options);
    }
}
