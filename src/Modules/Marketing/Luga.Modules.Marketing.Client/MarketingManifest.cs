using Luga.BuildingBlocks.Client.Manifests;

using MudBlazor;

namespace Luga.Modules.Marketing.Client;

/// <summary>
/// Marketing module manifest. Always visible (no subscription gate) and orders
/// itself before everything else so the public site shows up at the top of the
/// nav when an anonymous visitor sees it.
/// </summary>
public sealed class MarketingManifest : IModuleManifest
{
    public string ModuleCode => "marketing";

    public string DisplayNameKey => "manifest.marketing.displayName";

    public string IconName => Icons.Material.Filled.Public;

    public int Order => 0;

    public string? RequiredSubscriptionModule => null;

    public IReadOnlyList<MenuItem> MenuItems =>
    [
        new(LabelKey: "manifest.marketing.menu.home", "/", Icons.Material.Filled.Home, 10),
        new(LabelKey: "manifest.marketing.menu.pricing", "/pricing", Icons.Material.Filled.PriceCheck, 20),
        new(LabelKey: "manifest.marketing.menu.modules", "/modules", Icons.Material.Filled.Apps, 30),
        new(LabelKey: "manifest.marketing.menu.about", "/about", Icons.Material.Filled.Info, 40),
        new(LabelKey: "manifest.marketing.menu.contact", "/contact", Icons.Material.Filled.Email, 50),
    ];

    public IReadOnlyList<DashboardWidget> Widgets => [];

    public IReadOnlyList<BreadcrumbRoute> BreadcrumbRoutes =>
    [
        new(
            "/pricing",
            [
                new BreadcrumbSegment("common.home", Href: "/", IconName: Icons.Material.Filled.Home),
                new BreadcrumbSegment("manifest.marketing.breadcrumb.pricing"),
            ]),
        new(
            "/modules",
            [
                new BreadcrumbSegment("common.home", Href: "/", IconName: Icons.Material.Filled.Home),
                new BreadcrumbSegment("manifest.marketing.breadcrumb.modules"),
            ]),
        new(
            "/about",
            [
                new BreadcrumbSegment("common.home", Href: "/", IconName: Icons.Material.Filled.Home),
                new BreadcrumbSegment("manifest.marketing.breadcrumb.about"),
            ]),
        new(
            "/contact",
            [
                new BreadcrumbSegment("common.home", Href: "/", IconName: Icons.Material.Filled.Home),
                new BreadcrumbSegment("manifest.marketing.breadcrumb.contact"),
            ]),
    ];

    public IReadOnlyList<EmbeddableComponent> EmbeddableComponents => [];
}
