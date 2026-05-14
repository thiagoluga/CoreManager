using Luga.BuildingBlocks.Client.Manifests;

using MudBlazor;

namespace Luga.Modules.Payments.Client;

/// <summary>
/// Payments manifest. The MVP only declares the menu shell — actual CRUD pages
/// (plans, subscriptions, invoices) ship in V1.1.
/// </summary>
public sealed class PaymentsManifest : IModuleManifest
{
    public string ModuleCode => "payments";

    public string DisplayNameKey => "manifest.payments.displayName";

    public string IconName => Icons.Material.Filled.Payments;

    public int Order => 300;

    public string? RequiredSubscriptionModule => "payments";

    public IReadOnlyList<MenuItem> MenuItems =>
    [
        new(LabelKey: "manifest.payments.menu.invoices", "/invoices", Icons.Material.Filled.Receipt, 10),
    ];

    public IReadOnlyList<DashboardWidget> Widgets => [];

    public IReadOnlyList<BreadcrumbRoute> BreadcrumbRoutes => [];

    public IReadOnlyList<EmbeddableComponent> EmbeddableComponents => [];
}
