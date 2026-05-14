namespace Luga.BuildingBlocks.Client.Manifests;

/// <summary>
/// A Razor component a module contributes to a named extension point (slot)
/// owned by another module — e.g. an "extra tab" added by the Documents module
/// to the Customer detail page.
/// </summary>
/// <remarks>
/// Reserved for V2+. The MVP host does not render extension points yet; only
/// the manifest field exists so module authors can start declaring them.
/// </remarks>
/// <param name="Id">Stable identifier.</param>
/// <param name="ComponentType">Razor component type to render in the slot.</param>
/// <param name="ExtensionPoint">Slot name agreed between the host module and the contributing module.</param>
public sealed record EmbeddableComponent(
    string Id,
    Type ComponentType,
    string ExtensionPoint);
