using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Luga.BuildingBlocks.Server.Observability;

/// <summary>
/// Exposes <c>/health/live</c> (liveness — fast, no dependencies) and
/// <c>/health/ready</c> (readiness — all registered health checks).
/// </summary>
public static class HealthChecksSetup
{
    /// <summary>Liveness endpoint.</summary>
    public const string LivePath = "/health/live";

    /// <summary>Readiness endpoint.</summary>
    public const string ReadyPath = "/health/ready";

    /// <summary>Registers the health check pipeline.</summary>
    public static IServiceCollection AddLugaHealthChecks(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddHealthChecks();
        return services;
    }

    /// <summary>Maps the liveness + readiness endpoints.</summary>
    public static IEndpointRouteBuilder MapLugaHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);
        endpoints.MapHealthChecks(LivePath, new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => false, // liveness ignores registered checks
        });
        endpoints.MapHealthChecks(ReadyPath);
        return endpoints;
    }
}
