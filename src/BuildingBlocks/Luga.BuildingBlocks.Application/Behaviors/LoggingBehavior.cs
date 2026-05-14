using System.Diagnostics;

using Luga.BuildingBlocks.Application.Abstractions;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Luga.BuildingBlocks.Application.Behaviors;

/// <summary>
/// Logs the start and completion of every MediatR request together with
/// tenant id and user id pulled from the ambient context. Failures are
/// logged with the exception; cancellations are logged at debug level.
/// </summary>
/// <remarks>
/// First behavior in the pipeline order (Logging → Validation → Idempotency → Performance).
/// </remarks>
public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger,
    ITenantContext tenantContext,
    ICurrentUser currentUser) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;
    private readonly ITenantContext _tenantContext = tenantContext;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        string requestName = typeof(TRequest).Name;
        Guid tenantId = _tenantContext.IsAuthenticated ? _tenantContext.TenantId : Guid.Empty;
        Guid userId = _currentUser.IsAuthenticated ? _currentUser.UserId : Guid.Empty;

        _logger.LogInformation(
            "Handling {RequestName} (TenantId={TenantId}, UserId={UserId})",
            requestName, tenantId, userId);

        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
            TResponse response = await next().ConfigureAwait(false);
            stopwatch.Stop();

            _logger.LogInformation(
                "Handled {RequestName} in {ElapsedMs}ms",
                requestName, stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            _logger.LogDebug(
                "Cancelled {RequestName} after {ElapsedMs}ms",
                requestName, stopwatch.ElapsedMilliseconds);
            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "Failed {RequestName} after {ElapsedMs}ms",
                requestName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
