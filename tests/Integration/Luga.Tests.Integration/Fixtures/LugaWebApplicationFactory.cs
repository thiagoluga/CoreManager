using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Luga.Tests.Integration.Fixtures;

/// <summary>
/// Customizes <c>Luga.Server.Host</c> for integration tests by pointing the
/// connection strings at the throwaway SQL Server container and forcing the
/// environment to <c>Testing</c> so auth/scopes can be neutralised when needed.
/// </summary>
public sealed class LugaWebApplicationFactory(string sqlConnectionString)
    : WebApplicationFactory<Program>
{
    private readonly string _sqlConnectionString = sqlConnectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = _sqlConnectionString,
                ["ConnectionStrings:Hangfire"] = _sqlConnectionString,
                ["EntraExternalId:Authority"] = "https://invalid-for-tests.example.com",
                ["EntraExternalId:Audience"] = "api://tests",
            });
        });
    }
}
