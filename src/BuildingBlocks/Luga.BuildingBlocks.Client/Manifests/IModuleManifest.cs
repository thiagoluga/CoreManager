namespace Luga.BuildingBlocks.Client.Manifests;

/// <summary>
/// A module's self-description for the Blazor host: navigation, dashboard widgets,
/// breadcrumbs and (V2+) extension-point contributions. Discovered via DI — each
/// module's <c>X.ClientModule</c> registers a singleton implementation, and the host
/// composes them without ever knowing which modules exist (CLAUDE.md §9).
/// </summary>
public interface IModuleManifest
{
    /// <summary>Stable short code matching the module's <c>ModuleCode</c> on the server.</summary>
    string ModuleCode { get; }

    /// <summary>i18n key for the module's display name in menus and headers.</summary>
    string DisplayNameKey { get; }

    /// <summary>MudBlazor icon shown next to the module's menu section.</summary>
    string IconName { get; }

    /// <summary>Sort order among module sections. Lower wins.</summary>
    int Order { get; }

    /// <summary>
    /// Subscription module code that must be active for the tenant before the
    /// manifest is shown. <c>null</c> = always visible (e.g. Core, Marketing).
    /// </summary>
    string? RequiredSubscriptionModule { get; }

    /// <summary>Top-level menu items contributed by the module.</summary>
    IReadOnlyList<MenuItem> MenuItems { get; }

    /// <summary>Dashboard widgets contributed by the module.</summary>
    IReadOnlyList<DashboardWidget> Widgets { get; }

    /// <summary>Breadcrumb routes contributed by the module.</summary>
    IReadOnlyList<BreadcrumbRoute> BreadcrumbRoutes { get; }

    /// <summary>Extension-point components (reserved for V2+; return empty in the MVP).</summary>
    IReadOnlyList<EmbeddableComponent> EmbeddableComponents { get; }
}
