using Microsoft.Extensions.Localization;

namespace Luga.BuildingBlocks.Client.Localization;

/// <summary>
/// Resolves a key by walking an ordered list of inner localizers and returning
/// the first non-missing match. If every inner localizer reports
/// <see cref="LocalizedString.ResourceNotFound"/>, the key itself is returned.
/// </summary>
/// <remarks>
/// PageBreadcrumb uses this to fuse the page localizer, the matched manifest's
/// localizer, and <see cref="SharedStrings"/>. <c>null</c> entries are ignored
/// so callers can pass an optional page localizer without branching.
/// </remarks>
public sealed class ChainedStringLocalizer : IStringLocalizer
{
    private readonly IReadOnlyList<IStringLocalizer> _inner;

    public ChainedStringLocalizer(params IStringLocalizer?[] inner)
    {
        ArgumentNullException.ThrowIfNull(inner);
        _inner = [.. inner.Where(l => l is not null)!];
    }

    public LocalizedString this[string name]
    {
        get
        {
            ArgumentException.ThrowIfNullOrEmpty(name);

            foreach (IStringLocalizer localizer in _inner)
            {
                LocalizedString result = localizer[name];
                if (!result.ResourceNotFound)
                {
                    return result;
                }
            }

            return new LocalizedString(name, name, resourceNotFound: true);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            ArgumentException.ThrowIfNullOrEmpty(name);

            foreach (IStringLocalizer localizer in _inner)
            {
                LocalizedString result = localizer[name, arguments];
                if (!result.ResourceNotFound)
                {
                    return result;
                }
            }

            return new LocalizedString(name, name, resourceNotFound: true);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
        _inner.SelectMany(l => l.GetAllStrings(includeParentCultures));
}
