namespace Luga.BuildingBlocks.Client.Manifests;

/// <summary>
/// One link in a breadcrumb trail.
/// </summary>
/// <param name="LabelKey">i18n key for the segment label.</param>
/// <param name="Href">Optional link target. When null, the segment is rendered as plain text (typically the current page).</param>
/// <param name="IconName">Optional MudBlazor icon to display alongside the label.</param>
/// <param name="Source">Static (i18n) or Dynamic (provided by the page).</param>
public sealed record BreadcrumbSegment(
    string LabelKey,
    string? Href = null,
    string? IconName = null,
    BreadcrumbSegmentSource Source = BreadcrumbSegmentSource.Static);
