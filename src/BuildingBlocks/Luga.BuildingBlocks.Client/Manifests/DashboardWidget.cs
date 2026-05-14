namespace Luga.BuildingBlocks.Client.Manifests;

/// <summary>
/// Declarative descriptor for a dashboard widget contributed by a module.
/// The dashboard host renders <see cref="ComponentType"/> in its assigned slot.
/// </summary>
/// <param name="Id">Stable widget identifier (used for personalization/reordering).</param>
/// <param name="TitleKey">i18n key for the widget's header.</param>
/// <param name="ComponentType">Razor component type implementing the widget. Must be parameterless.</param>
/// <param name="Order">Sort order within the dashboard. Lower wins.</param>
/// <param name="Size">Grid footprint.</param>
/// <param name="RequiredPermission">When set, the widget is hidden unless the user has the permission.</param>
public sealed record DashboardWidget(
    string Id,
    string TitleKey,
    Type ComponentType,
    int Order,
    DashboardWidgetSize Size,
    string? RequiredPermission = null);
