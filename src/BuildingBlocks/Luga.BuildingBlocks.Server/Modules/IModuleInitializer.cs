namespace Luga.BuildingBlocks.Server.Modules;

/// <summary>
/// Seeds initial data for a module. Distinct from EF migrations (DDL): initializers
/// own DML (CLAUDE.md §7.11).
/// </summary>
/// <remarks>
/// The runner tracks the latest <see cref="Version"/> applied per
/// <see cref="ModuleCode"/> in <c>core.module_initializations</c> and only runs the
/// delta for new versions. Initializers MUST be idempotent within their own version.
/// </remarks>
public interface IModuleInitializer
{
    /// <summary>Stable short code of the owning module (e.g. <c>core</c>, <c>customers</c>).</summary>
    string ModuleCode { get; }

    /// <summary>
    /// Monotonically increasing version. Bumping invalidates the previous record and
    /// re-runs <see cref="InitializeAsync"/>.
    /// </summary>
    int Version { get; }

    /// <summary>Performs the seed using services resolved through <paramref name="context"/>.</summary>
    Task InitializeAsync(InitializationContext context, CancellationToken cancellationToken);
}
