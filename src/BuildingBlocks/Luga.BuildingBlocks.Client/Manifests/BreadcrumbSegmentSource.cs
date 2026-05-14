namespace Luga.BuildingBlocks.Client.Manifests;

/// <summary>
/// Where a breadcrumb segment's label comes from.
/// </summary>
public enum BreadcrumbSegmentSource
{
    /// <summary>Label is a static i18n key resolved at render time.</summary>
    Static = 0,

    /// <summary>
    /// Label is filled in dynamically by the page (e.g. "Customer John Smith") via the
    /// <c>DynamicLeaf</c> parameter on <c>PageBreadcrumb</c>.
    /// </summary>
    Dynamic = 1,
}
