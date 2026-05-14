using Luga.BuildingBlocks.Client.Manifests;

using MudBlazor;

namespace Luga.Modules.Personalization.Client;

/// <summary>
/// Personalization manifest — empty menu in the MVP (admin pages ship in V1.1).
/// The manifest still registers so the module is discoverable in the host.
/// </summary>
public sealed class PersonalizationManifest : IModuleManifest
{
    public string ModuleCode => "personalization";

    public string DisplayNameKey => "manifest.personalization.displayName";

    public string IconName => Icons.Material.Filled.Tune;

    public int Order => 900;

    public string? RequiredSubscriptionModule => "personalization";

    public IReadOnlyList<MenuItem> MenuItems => [];

    public IReadOnlyList<DashboardWidget> Widgets => [];

    public IReadOnlyList<BreadcrumbRoute> BreadcrumbRoutes => [];

    public IReadOnlyList<EmbeddableComponent> EmbeddableComponents => [];
}
