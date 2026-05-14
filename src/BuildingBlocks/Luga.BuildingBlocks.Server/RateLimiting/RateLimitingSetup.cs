using System.Threading.RateLimiting;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Luga.BuildingBlocks.Server.RateLimiting;

/// <summary>
/// Minimal rate-limiting policy: 100 requests per minute per partition. The
/// partition is the authenticated tenant id when available, otherwise the
/// remote IP. CLAUDE.md §16 — covers basic abuse defense; more granular
/// per-endpoint policies come with V1.1.
/// </summary>
public static class RateLimitingSetup
{
    public const string GlobalPolicyName = "Luga.Global";

    public static IServiceCollection AddLugaRateLimiting(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                string partitionKey = ResolvePartitionKey(context);
                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0,
                        AutoReplenishment = true,
                    });
            });
        });

        return services;
    }

    private static string ResolvePartitionKey(HttpContext context)
    {
        string? tenantId = context.User.FindFirst("tenant_id")?.Value;
        if (!string.IsNullOrWhiteSpace(tenantId))
        {
            return $"tenant:{tenantId}";
        }

        string? ip = context.Connection.RemoteIpAddress?.ToString();
        return $"ip:{ip ?? "unknown"}";
    }
}
