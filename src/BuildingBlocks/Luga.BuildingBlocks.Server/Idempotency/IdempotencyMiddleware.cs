using System.Text;

using Luga.BuildingBlocks.Application.Abstractions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Luga.BuildingBlocks.Server.Idempotency;

/// <summary>
/// Reads the <c>Idempotency-Key</c> header on mutating requests and either
/// short-circuits with the cached payload or runs the request and stores the
/// response for future retries (CLAUDE.md §16).
/// </summary>
/// <remarks>
/// Only applies to POST/PUT/PATCH/DELETE. GETs are inherently idempotent.
/// </remarks>
public sealed class IdempotencyMiddleware(RequestDelegate next)
{
    private const string HeaderName = "Idempotency-Key";

    private static readonly HashSet<string> MutatingMethods =
        new(StringComparer.OrdinalIgnoreCase) { "POST", "PUT", "PATCH", "DELETE" };

    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(
        HttpContext context,
        IIdempotencyStore store,
        ILogger<IdempotencyMiddleware> logger)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(logger);

        if (!MutatingMethods.Contains(context.Request.Method) ||
            !context.Request.Headers.TryGetValue(HeaderName, out Microsoft.Extensions.Primitives.StringValues keyValues))
        {
            await _next(context).ConfigureAwait(false);
            return;
        }

        string? key = keyValues.ToString();
        if (string.IsNullOrWhiteSpace(key))
        {
            await _next(context).ConfigureAwait(false);
            return;
        }

        string? cached = await store.TryGetAsync(key, context.RequestAborted).ConfigureAwait(false);
        if (!string.IsNullOrEmpty(cached))
        {
            logger.LogInformation("Idempotency-Key {Key} short-circuit", key);
            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(cached, Encoding.UTF8, context.RequestAborted).ConfigureAwait(false);
            return;
        }

        // Buffer the response body so we can both write to the client and capture for storage.
        Stream originalBody = context.Response.Body;
        await using MemoryStream buffer = new();
        context.Response.Body = buffer;

        try
        {
            await _next(context).ConfigureAwait(false);

            if (context.Response.StatusCode is >= 200 and < 300)
            {
                buffer.Position = 0;
                using StreamReader reader = new(buffer, Encoding.UTF8, leaveOpen: true);
                string payload = await reader.ReadToEndAsync(context.RequestAborted).ConfigureAwait(false);
                await store.SaveAsync(key, payload, expiresIn: null, context.RequestAborted).ConfigureAwait(false);
            }
        }
        finally
        {
            buffer.Position = 0;
            await buffer.CopyToAsync(originalBody, context.RequestAborted).ConfigureAwait(false);
            context.Response.Body = originalBody;
        }
    }
}
