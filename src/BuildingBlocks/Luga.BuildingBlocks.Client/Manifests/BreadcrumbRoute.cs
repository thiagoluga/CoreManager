namespace Luga.BuildingBlocks.Client.Manifests;

/// <summary>
/// Maps a Blazor route pattern to the breadcrumb segments shown on that page.
/// </summary>
/// <param name="RoutePattern">Blazor route template (e.g. <c>/customers</c>, <c>/customers/{id:guid}</c>).</param>
/// <param name="Segments">Ordered breadcrumb trail. Convention: first segment is the home crumb.</param>
/// <param name="IsEnabled">Toggle for development/feature flags. Disabled routes are ignored at render time.</param>
public sealed record BreadcrumbRoute(
    string RoutePattern,
    IReadOnlyList<BreadcrumbSegment> Segments,
    bool IsEnabled = true);
