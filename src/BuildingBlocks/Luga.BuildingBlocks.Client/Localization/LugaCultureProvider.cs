using System.Globalization;

namespace Luga.BuildingBlocks.Client.Localization;

/// <summary>
/// Default in-memory <see cref="ILugaCultureProvider"/>. Configured from
/// <see cref="LocalizationOptions"/>.
/// </summary>
public sealed class LugaCultureProvider : ILugaCultureProvider
{
    private readonly CultureInfo _fallback;
    private readonly IReadOnlyList<CultureInfo> _supported;

    public LugaCultureProvider(LocalizationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _supported = [.. options.SupportedCultures];
        _fallback = options.FallbackCulture;
    }

    /// <inheritdoc/>
    public IReadOnlyList<CultureInfo> SupportedCultures => _supported;

    /// <inheritdoc/>
    public CultureInfo FallbackCulture => _fallback;

    /// <inheritdoc/>
    public CultureInfo Resolve(string? userPreferred, string? tenantDefault, string? browserPreferred)
    {
        return Match(userPreferred)
            ?? Match(tenantDefault)
            ?? Match(browserPreferred)
            ?? _fallback;
    }

    private CultureInfo? Match(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return _supported.FirstOrDefault(c =>
            string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
    }
}
