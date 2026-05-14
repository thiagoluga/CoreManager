namespace Luga.BuildingBlocks.Infrastructure.Persistence.Migrations;

/// <summary>
/// Row in <c>core.module_initializations</c>. Tracks which version of each module's
/// initializer has been applied so the runner only re-executes when the version bumps.
/// </summary>
public sealed class ModuleInitialization
{
    /// <summary>Module short code (e.g. <c>core</c>, <c>customers</c>).</summary>
    public string ModuleCode { get; set; } = string.Empty;

    /// <summary>Version successfully applied.</summary>
    public int Version { get; set; }

    /// <summary>UTC timestamp when the initializer completed.</summary>
    public DateTime AppliedAt { get; set; }

    /// <summary>Hostname / actor that ran the initializer.</summary>
    public string AppliedBy { get; set; } = string.Empty;
}
