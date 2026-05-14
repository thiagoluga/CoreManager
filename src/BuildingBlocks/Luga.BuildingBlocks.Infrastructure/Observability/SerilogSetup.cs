using System.Globalization;

using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Events;

namespace Luga.BuildingBlocks.Infrastructure.Observability;

/// <summary>
/// Minimal Serilog wiring: structured console output with environment + thread
/// enrichers and reading additional sinks from configuration.
/// </summary>
/// <remarks>
/// Application Insights sink is added by <c>OpenTelemetrySetup</c> (so we ship a
/// single trace/log pipeline). Seq is opt-in via appsettings for dev convenience.
/// </remarks>
public static class SerilogSetup
{
    /// <summary>
    /// Replaces the default logging stack with Serilog. Call once during host setup.
    /// </summary>
    public static IHostBuilder UseLugaSerilog(this IHostBuilder hostBuilder)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);

        return hostBuilder.UseSerilog((context, _, logger) => logger
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
            .WriteTo.Console(
                restrictedToMinimumLevel: LogEventLevel.Information,
                formatProvider: CultureInfo.InvariantCulture));
    }
}
