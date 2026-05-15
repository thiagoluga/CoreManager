using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Luga.Tests.Integration.Fixtures;

/// <summary>
/// Customizes <c>Luga.Server.Host</c> for integration tests by pointing the
/// connection strings at the throwaway SQL Server container and forcing the
/// environment to <c>Testing</c> so auth/scopes can be neutralised when needed.
/// </summary>
/// <remarks>
/// We override the connection strings via process-scoped environment variables
/// instead of <see cref="IWebHostBuilder.ConfigureAppConfiguration"/> because
/// <c>Luga.Server.Host</c>'s <c>Program.cs</c> reads
/// <c>builder.Configuration["ConnectionStrings:Hangfire"]</c> at the top of
/// <c>WebApplication.CreateBuilder(args)</c> — the test factory's
/// <c>ConfigureAppConfiguration</c> callback only runs <em>after</em> that read,
/// so any in-memory overrides arrive too late and Hangfire ends up resolving
/// the value from <c>appsettings.json</c> (LocalDB on this repo). Env vars are
/// merged into <see cref="IConfiguration"/> by the default
/// <c>WebApplication.CreateBuilder</c> sources and therefore win over
/// <c>appsettings.json</c> the moment Program reads them.
/// </remarks>
public sealed class LugaWebApplicationFactory : WebApplicationFactory<Program>
{
    private static readonly string[] OverriddenEnvironmentKeys =
    [
        "ConnectionStrings__Default",
        "ConnectionStrings__Hangfire",
        "EntraExternalId__Authority",
        "EntraExternalId__Audience",
        "ApplyMigrationsOnStartup",
    ];

    private readonly string _sqlConnectionString;

    public LugaWebApplicationFactory(string sqlConnectionString)
    {
        _sqlConnectionString = sqlConnectionString;

        Environment.SetEnvironmentVariable("ConnectionStrings__Default", sqlConnectionString);
        Environment.SetEnvironmentVariable("ConnectionStrings__Hangfire", sqlConnectionString);
        Environment.SetEnvironmentVariable("EntraExternalId__Authority", "https://invalid-for-tests.example.com");
        Environment.SetEnvironmentVariable("EntraExternalId__Audience", "api://tests");
        // Smoke tests hit `/health/live` only — no schema needed. Disabling
        // startup migrations keeps the test fast and side-effect-free.
        Environment.SetEnvironmentVariable("ApplyMigrationsOnStartup", "false");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.UseEnvironment("Testing");

        // Belt-and-braces: also project the values via the in-memory provider
        // so anything resolved from `IConfiguration` later in the pipeline still
        // sees the overrides even if the env-var source has been pruned.
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = _sqlConnectionString,
                ["ConnectionStrings:Hangfire"] = _sqlConnectionString,
                ["EntraExternalId:Authority"] = "https://invalid-for-tests.example.com",
                ["EntraExternalId:Audience"] = "api://tests",
                ["ApplyMigrationsOnStartup"] = "false",
            });
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            // Avoid leaking the test connection string into other test fixtures
            // that may run later in the same process.
            foreach (string key in OverriddenEnvironmentKeys)
            {
                Environment.SetEnvironmentVariable(key, value: null);
            }
        }
    }
}
