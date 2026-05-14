namespace Luga.BuildingBlocks.Client.Tenancy;

/// <summary>
/// Client-side snapshot of the tenant the user is signed in to. Populated by the
/// host after the user authenticates (typically from <c>GET /api/users/me</c>)
/// and exposed both as a scoped DI service and as a cascading value so pages can
/// branch on tenant-level state without an extra round-trip.
/// </summary>
/// <remarks>
/// This is the front-end mirror of <c>Application.Abstractions.ITenantContext</c>:
/// the server reads tenant data from the JWT; the client gets a richer shape from
/// the API (e.g. list of active modules).
/// </remarks>
public sealed class TenantContext
{
    /// <summary>Sentinel returned when no tenant has been resolved yet (e.g. on public pages).</summary>
    public static TenantContext Anonymous { get; } = new();

    /// <summary>Tenant id.</summary>
    public Guid TenantId { get; init; }

    /// <summary>URL-safe slug.</summary>
    public string Slug { get; init; } = string.Empty;

    /// <summary>Tenant display name shown in headers.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Tenant default UI culture (e.g. <c>pt-BR</c>).</summary>
    public string DefaultCulture { get; init; } = "pt-BR";

    /// <summary>Culture used to format money values. Brazilian tenants are always <c>pt-BR</c>.</summary>
    public string MoneyCulture { get; init; } = "pt-BR";

    /// <summary>Module codes currently active for the tenant (e.g. <c>customers</c>, <c>payments</c>).</summary>
    public IReadOnlySet<string> ActiveModules { get; init; } = new HashSet<string>(StringComparer.Ordinal);

    /// <summary>True when the given module is part of the tenant's subscription.</summary>
    public bool HasModuleActive(string moduleCode) =>
        !string.IsNullOrWhiteSpace(moduleCode) && ActiveModules.Contains(moduleCode);
}
