using Luga.BuildingBlocks.Server.Modules;

namespace Luga.Modules.Core.Server;

/// <summary>
/// Seeds the platform-wide baseline (CLAUDE.md §7.11). Right now the Core
/// module owns no static seed data — the file exists as the canonical hook for
/// future bumps (e.g. Pricing plan catalog in §6.2, default RBAC roles in §6.4).
/// Bumping <see cref="Version"/> re-runs <see cref="InitializeAsync"/>.
/// </summary>
public sealed class CoreModuleInitializer : IModuleInitializer
{
    /// <inheritdoc/>
    public string ModuleCode => "core";

    /// <inheritdoc/>
    public int Version => 1;

    /// <inheritdoc/>
    public Task InitializeAsync(InitializationContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Intentionally empty in the MVP — see class summary.
        return Task.CompletedTask;
    }
}
