using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Luga.BuildingBlocks.Server.Tenancy;

/// <summary>
/// Enriches the log scope with the resolved tenant id so every log entry for
/// the request carries it. The <see cref="HttpTenantContext"/> handles the actual
/// resolution lazily on demand.
/// </summary>
public sealed class TenantContextMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, ILogger<TenantContextMiddleware> logger)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(logger);

        Guid? tenantId = TenantClaimsExtractor.GetTenantId(context.User);

        using IDisposable? scope = tenantId is null
            ? null
            : logger.BeginScope(new Dictionary<string, object> { ["TenantId"] = tenantId.Value });

        await _next(context).ConfigureAwait(false);
    }
}
