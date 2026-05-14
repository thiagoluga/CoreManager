using Luga.BuildingBlocks.Client.Manifests;

using MudBlazor;

namespace Luga.Modules.Core.Client;

/// <summary>
/// Manifest declaring the Core module's UI surface — profile, tenant settings,
/// admin shells. Core is always visible (no <see cref="IModuleManifest.RequiredSubscriptionModule"/>).
/// </summary>
public sealed class CoreManifest : IModuleManifest
{
    /// <inheritdoc/>
    public string ModuleCode => "core";

    /// <inheritdoc/>
    public string DisplayNameKey => "manifest.core.displayName";

    /// <inheritdoc/>
    public string IconName => Icons.Material.Filled.Settings;

    /// <inheritdoc/>
    public int Order => 100;

    /// <inheritdoc/>
    public string? RequiredSubscriptionModule => null;

    /// <inheritdoc/>
    public IReadOnlyList<MenuItem> MenuItems =>
    [
        new(
            LabelKey: "manifest.core.menu.profile",
            Route: "/profile",
            IconName: Icons.Material.Filled.AccountCircle,
            Order: 10),
        new(
            LabelKey: "manifest.core.menu.settings",
            Route: "/settings",
            IconName: Icons.Material.Filled.Tune,
            Order: 20),
    ];

    /// <inheritdoc/>
    public IReadOnlyList<DashboardWidget> Widgets => [];

    /// <inheritdoc/>
    public IReadOnlyList<BreadcrumbRoute> BreadcrumbRoutes =>
    [
        new BreadcrumbRoute(
            "/profile",
            [
                new BreadcrumbSegment("common.home", Href: "/", IconName: Icons.Material.Filled.Home),
                new BreadcrumbSegment("manifest.core.breadcrumb.profile"),
            ]),
        new BreadcrumbRoute(
            "/settings",
            [
                new BreadcrumbSegment("common.home", Href: "/", IconName: Icons.Material.Filled.Home),
                new BreadcrumbSegment("manifest.core.breadcrumb.settings"),
            ]),
    ];

    /// <inheritdoc/>
    public IReadOnlyList<EmbeddableComponent> EmbeddableComponents => [];
}
