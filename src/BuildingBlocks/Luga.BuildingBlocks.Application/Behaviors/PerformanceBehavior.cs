using System.Diagnostics;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Luga.BuildingBlocks.Application.Behaviors;

/// <summary>
/// Emits a warning when a request takes longer than <see cref="SlowRequestThresholdMs"/>.
/// Cheap baseline before we wire OpenTelemetry traces per handler.
/// </summary>
/// <remarks>
/// Last behavior in the pipeline (after Logging, Validation, Idempotency).
/// </remarks>
public sealed class PerformanceBehavior<TRequest, TResponse>(
    ILogger<PerformanceBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>Threshold (in milliseconds) above which a request is logged as slow.</summary>
    public const int SlowRequestThresholdMs = 500;

    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        Stopwatch stopwatch = Stopwatch.StartNew();
        TResponse response = await next().ConfigureAwait(false);
        stopwatch.Stop();

        if (stopwatch.ElapsedMilliseconds > SlowRequestThresholdMs)
        {
            _logger.LogWarning(
                "Slow request {RequestName} took {ElapsedMs}ms (threshold {ThresholdMs}ms)",
                typeof(TRequest).Name, stopwatch.ElapsedMilliseconds, SlowRequestThresholdMs);
        }

        return response;
    }
}
