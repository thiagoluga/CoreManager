using System.Globalization;

namespace Luga.BuildingBlocks.Client.Localization;

/// <summary>
/// Strongly-typed options for the client-side localization pipeline.
/// The MVP ships pt-BR; en-US and es-ES are pre-wired with placeholder
/// resources for V2+ (CLAUDE.md §11.3, §11.10).
/// </summary>
public sealed class LocalizationOptions
{
    private CultureInfo? _fallbackCulture;

    /// <summary>Cultures the app currently exposes (default: pt-BR, en-US, es-ES).</summary>
    public IList<CultureInfo> SupportedCultures { get; } =
    [
        new CultureInfo("pt-BR"),
        new CultureInfo("en-US"),
        new CultureInfo("es-ES"),
    ];

    /// <summary>Fallback when no other source resolves a supported culture.</summary>
    public CultureInfo FallbackCulture
    {
        get => _fallbackCulture ?? new CultureInfo("pt-BR");
        set => _fallbackCulture = value;
    }

    /// <summary>Folder (relative to assembly) where the JSON resource files live.</summary>
    public string ResourcesPath { get; set; } = "Resources";
}
