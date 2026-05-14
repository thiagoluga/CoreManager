using Luga.BuildingBlocks.Client.Manifests;

using MudBlazor;

namespace Luga.Modules.Customers.Client;

public sealed class CustomersManifest : IModuleManifest
{
    public string ModuleCode => "customers";

    public string DisplayNameKey => "manifest.customers.displayName";

    public string IconName => Icons.Material.Filled.People;

    public int Order => 200;

    public string? RequiredSubscriptionModule => "customers";

    public IReadOnlyList<MenuItem> MenuItems =>
    [
        new(LabelKey: "manifest.customers.menu.list", "/customers", Icons.Material.Filled.People, 10),
        new(LabelKey: "manifest.customers.menu.new", "/customers/new", Icons.Material.Filled.PersonAdd, 20),
    ];

    public IReadOnlyList<DashboardWidget> Widgets => [];

    public IReadOnlyList<BreadcrumbRoute> BreadcrumbRoutes =>
    [
        new(
            "/customers",
            [
                new BreadcrumbSegment("common.home", Href: "/", IconName: Icons.Material.Filled.Home),
                new BreadcrumbSegment("manifest.customers.breadcrumb.list"),
            ]),
        new(
            "/customers/new",
            [
                new BreadcrumbSegment("common.home", Href: "/", IconName: Icons.Material.Filled.Home),
                new BreadcrumbSegment("manifest.customers.breadcrumb.list", Href: "/customers"),
                new BreadcrumbSegment("manifest.customers.breadcrumb.new"),
            ]),
        new(
            "/customers/{id:guid}",
            [
                new BreadcrumbSegment("common.home", Href: "/", IconName: Icons.Material.Filled.Home),
                new BreadcrumbSegment("manifest.customers.breadcrumb.list", Href: "/customers"),
                new BreadcrumbSegment("manifest.customers.breadcrumb.detail", Source: BreadcrumbSegmentSource.Dynamic),
            ]),
    ];

    public IReadOnlyList<EmbeddableComponent> EmbeddableComponents => [];
}
