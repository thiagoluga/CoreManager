using Luga.BuildingBlocks.Client.Manifests;

namespace Luga.BuildingBlocks.Client.Navigation;

/// <summary>
/// Picks the breadcrumb trail for the current Blazor route, fusing static
/// declarations from <see cref="IModuleManifest.BreadcrumbRoutes"/> with the
/// page-supplied dynamic leaf (e.g. the customer's name on a detail page).
/// </summary>
public interface IBreadcrumbResolver
{
    /// <summary>
    /// Returns the segments to render. When no manifest matches, returns an
    /// empty list — callers typically skip rendering in that case.
    /// </summary>
    /// <param name="currentRoute">Current route (e.g. <c>/customers</c>). Path only — no query string.</param>
    /// <param name="dynamicLeafLabel">
    /// Label supplied by the page for the last <see cref="BreadcrumbSegmentSource.Dynamic"/>
    /// segment. <c>null</c> means use the static template.
    /// </param>
    IReadOnlyList<BreadcrumbSegment> Resolve(string currentRoute, string? dynamicLeafLabel);

    /// <summary>
    /// Like <see cref="Resolve"/> but also returns the manifest that owns the
    /// matched route. PageBreadcrumb uses the manifest's type to localize
    /// <c>manifest.*</c> label keys against the right resource file.
    /// </summary>
    BreadcrumbMatch? ResolveMatch(string currentRoute, string? dynamicLeafLabel);
}

/// <summary>Pair of segments + the manifest whose route matched.</summary>
public sealed record BreadcrumbMatch(
    IReadOnlyList<BreadcrumbSegment> Segments,
    IModuleManifest Manifest);
