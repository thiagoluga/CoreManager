using Luga.BuildingBlocks.Infrastructure.Persistence;
using Luga.BuildingBlocks.Infrastructure.Persistence.Migrations;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Luga.BuildingBlocks.Server.Modules;

/// <summary>
/// Discovers all registered <see cref="IModuleInitializer"/> instances and runs
/// them in <see cref="IModuleInitializer.ModuleCode"/> order, applying only the
/// versions newer than what is recorded in <c>core.module_initializations</c>
/// (CLAUDE.md §7.11). The tracking table belongs to a context that maps
/// <see cref="ModuleInitialization"/>.
/// </summary>
public sealed class ModuleInitializerRunner(
    IServiceScopeFactory scopeFactory,
    TimeProvider timeProvider,
    ILogger<ModuleInitializerRunner> logger)
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly ILogger<ModuleInitializerRunner> _logger = logger;

    /// <summary>
    /// Runs every initializer whose <see cref="IModuleInitializer.Version"/> is greater
    /// than the latest version recorded in <paramref name="trackingContextType"/>.
    /// </summary>
    /// <param name="trackingContextType">
    /// The <see cref="LugaDbContextBase"/> that maps <see cref="ModuleInitialization"/>.
    /// Typically the Core module's DbContext.
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task RunAsync(Type trackingContextType, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(trackingContextType);

        await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();

        if (scope.ServiceProvider.GetRequiredService(trackingContextType) is not LugaDbContextBase tracking)
        {
            throw new InvalidOperationException(
                $"Tracking context '{trackingContextType.FullName}' is not a LugaDbContextBase.");
        }

        IEnumerable<IModuleInitializer> initializers = scope.ServiceProvider
            .GetServices<IModuleInitializer>()
            .OrderBy(i => i.ModuleCode, StringComparer.OrdinalIgnoreCase)
            .ThenBy(i => i.Version);

        DbSet<ModuleInitialization> trackingSet = tracking.Set<ModuleInitialization>();
        string appliedBy = Environment.MachineName;

        foreach (IModuleInitializer initializer in initializers)
        {
            int latestApplied = await trackingSet
                .Where(m => m.ModuleCode == initializer.ModuleCode)
                .OrderByDescending(m => m.Version)
                .Select(m => (int?)m.Version)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false) ?? 0;

            if (initializer.Version <= latestApplied)
            {
                _logger.LogDebug(
                    "Skipping initializer {Module} v{Version} (already at v{Latest})",
                    initializer.ModuleCode, initializer.Version, latestApplied);
                continue;
            }

            _logger.LogInformation(
                "Running initializer {Module} v{Version}",
                initializer.ModuleCode, initializer.Version);

            InitializationContext context = new(scope.ServiceProvider);
            await initializer.InitializeAsync(context, cancellationToken).ConfigureAwait(false);

            trackingSet.Add(new ModuleInitialization
            {
                ModuleCode = initializer.ModuleCode,
                Version = initializer.Version,
                AppliedAt = _timeProvider.GetUtcNow().UtcDateTime,
                AppliedBy = appliedBy,
            });

            await tracking.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
