using Hangfire;
using Hangfire.SqlServer;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Luga.BuildingBlocks.Server.BackgroundJobs;

/// <summary>
/// Registers Hangfire (SQL Server storage) and exposes the dashboard at <c>/jobs</c>
/// behind an authorization policy that requires the <c>jobs.dashboard</c> permission.
/// </summary>
public static class HangfireSetup
{
    /// <summary>Configuration key holding the Hangfire SQL connection string.</summary>
    public const string ConnectionStringKey = "ConnectionStrings:Hangfire";

    /// <summary>Dashboard route.</summary>
    public const string DashboardPath = "/jobs";

    /// <summary>Authorization policy required for accessing the dashboard.</summary>
    public const string DashboardPolicyName = "HangfireDashboard";

    /// <summary>Registers Hangfire services with SQL Server storage.</summary>
    public static IServiceCollection AddLugaHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        string connection = configuration[ConnectionStringKey]
            ?? configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException(
                $"Connection string missing. Expected '{ConnectionStringKey}' or 'ConnectionStrings:Default'.");

        services.AddHangfire(cfg => cfg
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connection, new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.FromSeconds(15),
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true,
            }));

        services.AddHangfireServer();

        services.AddAuthorizationBuilder()
            .AddPolicy(DashboardPolicyName, policy => policy.RequireAuthenticatedUser());

        return services;
    }

    /// <summary>Maps the Hangfire dashboard endpoint at <see cref="DashboardPath"/>.</summary>
    public static IEndpointRouteBuilder MapLugaHangfireDashboard(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        endpoints.MapHangfireDashboard(DashboardPath, new DashboardOptions
        {
            DashboardTitle = "Luga Jobs",
            DisplayStorageConnectionString = false,
        }).RequireAuthorization(DashboardPolicyName);

        return endpoints;
    }
}
