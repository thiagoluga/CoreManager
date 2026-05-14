using System.Text.RegularExpressions;

using Luga.BuildingBlocks.Client.Manifests;

namespace Luga.BuildingBlocks.Client.Navigation;

/// <summary>
/// Default in-memory <see cref="IBreadcrumbResolver"/>. Snapshots the breadcrumb
/// routes from all manifests at construction; manifests are singletons, so no
/// runtime mutation is expected.
/// </summary>
/// <remarks>
/// Route matching strips template parameters (<c>{id:guid}</c>, <c>{slug?}</c>),
/// then matches segment-by-segment against the live URL. UI/URL overrides are
/// V2+ scope (CLAUDE.md §10).
/// </remarks>
public sealed partial class BreadcrumbResolver : IBreadcrumbResolver
{
    private readonly IReadOnlyList<BreadcrumbRoute> _routes;

    public BreadcrumbResolver(IEnumerable<IModuleManifest> manifests)
    {
        ArgumentNullException.ThrowIfNull(manifests);
        _routes = [.. manifests.SelectMany(m => m.BreadcrumbRoutes).Where(r => r.IsEnabled)];
    }

    /// <inheritdoc/>
    public IReadOnlyList<BreadcrumbSegment> Resolve(string currentRoute, string? dynamicLeafLabel)
    {
        ArgumentException.ThrowIfNullOrEmpty(currentRoute);

        BreadcrumbRoute? match = _routes.FirstOrDefault(r => RouteMatches(r.RoutePattern, currentRoute));
        if (match is null)
        {
            return [];
        }

        if (string.IsNullOrWhiteSpace(dynamicLeafLabel))
        {
            return match.Segments;
        }

        // Replace the label key of the last Dynamic segment (if any) with the supplied label.
        List<BreadcrumbSegment> segments = [.. match.Segments];
        for (int i = segments.Count - 1; i >= 0; i--)
        {
            if (segments[i].Source == BreadcrumbSegmentSource.Dynamic)
            {
                segments[i] = segments[i] with { LabelKey = dynamicLeafLabel };
                break;
            }
        }

        return segments;
    }

    private static bool RouteMatches(string pattern, string currentRoute)
    {
        // Naive segment-by-segment match: '{name[:constraint][?]}' wildcards match any non-empty segment.
        string[] patternSegments = pattern.Trim('/').Split('/');
        string[] urlSegments = currentRoute.Trim('/').Split('/');

        if (patternSegments.Length != urlSegments.Length)
        {
            return false;
        }

        for (int i = 0; i < patternSegments.Length; i++)
        {
            if (ParameterPattern().IsMatch(patternSegments[i]))
            {
                if (string.IsNullOrEmpty(urlSegments[i]))
                {
                    return false;
                }

                continue;
            }

            if (!string.Equals(patternSegments[i], urlSegments[i], StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    [GeneratedRegex(@"\{[^}]+\}", RegexOptions.CultureInvariant)]
    private static partial Regex ParameterPattern();
}
