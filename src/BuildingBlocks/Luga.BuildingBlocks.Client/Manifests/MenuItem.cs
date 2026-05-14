namespace Luga.BuildingBlocks.Client.Manifests;

/// <summary>
/// One row in a module's navigation menu.
/// </summary>
/// <param name="LabelKey">i18n key resolved by the module's resources (no literal strings).</param>
/// <param name="Route">Blazor route the item navigates to.</param>
/// <param name="IconName">MudBlazor icon name (e.g. <c>Icons.Material.Filled.People</c>).</param>
/// <param name="Order">Sort order within the section. Lower wins.</param>
/// <param name="RequiredPermission">When set, the item is hidden unless the user has the permission.</param>
/// <param name="Children">Optional nested menu items (single-level nesting in the MVP).</param>
public sealed record MenuItem(
    string LabelKey,
    string Route,
    string IconName,
    int Order,
    string? RequiredPermission = null,
    IReadOnlyList<MenuItem>? Children = null);
