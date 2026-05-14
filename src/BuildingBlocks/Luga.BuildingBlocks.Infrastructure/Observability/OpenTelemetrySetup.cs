using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Luga.BuildingBlocks.Infrastructure.Observability;

/// <summary>
/// Wires OpenTelemetry traces and metrics. Adds correlation-id propagation
/// so log entries, traces, and downstream HTTP calls share an Activity id
/// (CLAUDE.md §3.4 perigo 6).
/// </summary>
/// <remarks>
/// Azure Application Insights exporter is configured separately by the host
/// (it requires the connection string from <c>appsettings.json</c>).
/// </remarks>
public static class OpenTelemetrySetup
{
    /// <summary>Adds OpenTelemetry traces + metrics with default instrumentations.</summary>
    public static IServiceCollection AddLugaOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        string serviceName = configuration["OpenTelemetry:ServiceName"] ?? "luga-coremanager";

        services.AddOpenTelemetry()
            .ConfigureResource(rb => rb.AddService(serviceName))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSource("Luga.*"))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation());

        return services;
    }
}
