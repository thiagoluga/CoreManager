using System.Globalization;

namespace Luga.BuildingBlocks.Client.Localization;

/// <summary>
/// Resolves the active UI culture using the cascade
/// <c>user preference → tenant default → browser → configured fallback</c>
/// (CLAUDE.md §11.7).
/// </summary>
public interface ILugaCultureProvider
{
    /// <summary>Cultures the app currently has resource files for.</summary>
    IReadOnlyList<CultureInfo> SupportedCultures { get; }

    /// <summary>Configured fallback when no other source resolves a supported culture.</summary>
    CultureInfo FallbackCulture { get; }

    /// <summary>Picks the active culture. Pure — does not mutate <c>CurrentUICulture</c>.</summary>
    CultureInfo Resolve(
        string? userPreferred,
        string? tenantDefault,
        string? browserPreferred);
}
