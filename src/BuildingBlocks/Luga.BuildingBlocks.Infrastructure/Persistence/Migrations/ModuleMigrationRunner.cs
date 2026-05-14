using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Luga.BuildingBlocks.Infrastructure.Persistence.Migrations;

/// <summary>
/// Applies pending EF migrations for every registered <c>LugaDbContextBase</c>.
/// Used in development startup (<c>ApplyMigrationsOnStartup=true</c>); production
/// runs migrations via a dedicated CI/CD job before the app deploys (CLAUDE.md §7.9).
/// </summary>
public sealed class ModuleMigrationRunner(
    IServiceScopeFactory scopeFactory,
    ILogger<ModuleMigrationRunner> logger)
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<ModuleMigrationRunner> _logger = logger;

    /// <summary>Applies all pending migrations for the given context types.</summary>
    public async Task RunAsync(IEnumerable<Type> dbContextTypes, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dbContextTypes);

        await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();

        foreach (Type contextType in dbContextTypes)
        {
            if (!typeof(LugaDbContextBase).IsAssignableFrom(contextType))
            {
                throw new InvalidOperationException(
                    $"Context type '{contextType.FullName}' is not a LugaDbContextBase.");
            }

            object service = scope.ServiceProvider.GetRequiredService(contextType);
            if (service is not LugaDbContextBase dbContext)
            {
                throw new InvalidOperationException(
                    $"Resolved service for '{contextType.FullName}' is not a LugaDbContextBase.");
            }

            _logger.LogInformation("Applying migrations for {Context}", contextType.Name);
            await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
