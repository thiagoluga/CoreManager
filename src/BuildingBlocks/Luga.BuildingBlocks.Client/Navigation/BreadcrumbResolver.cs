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
    private readonly IReadOnlyList<RouteEntry> _routes;

    public BreadcrumbResolver(IEnumerable<IModuleManifest> manifests)
    {
        ArgumentNullException.ThrowIfNull(manifests);
        _routes =
        [
            .. manifests.SelectMany(m => m.BreadcrumbRoutes.Where(r => r.IsEnabled).Select(r => new RouteEntry(m, r))),
        ];
    }

    /// <inheritdoc/>
    public IReadOnlyList<BreadcrumbSegment> Resolve(string currentRoute, string? dynamicLeafLabel) =>
        ResolveMatch(currentRoute, dynamicLeafLabel)?.Segments ?? [];

    /// <inheritdoc/>
    public BreadcrumbMatch? ResolveMatch(string currentRoute, string? dynamicLeafLabel)
    {
        ArgumentException.ThrowIfNullOrEmpty(currentRoute);

        RouteEntry? match = _routes.FirstOrDefault(e => RouteMatches(e.Route.RoutePattern, currentRoute));
        if (match is null)
        {
            return null;
        }

        IReadOnlyList<BreadcrumbSegment> segments = match.Route.Segments;

        if (!string.IsNullOrWhiteSpace(dynamicLeafLabel))
        {
            // Replace the label key of the last Dynamic segment (if any) with the supplied label.
            List<BreadcrumbSegment> mutable = [.. segments];
            for (int i = mutable.Count - 1; i >= 0; i--)
            {
                if (mutable[i].Source == BreadcrumbSegmentSource.Dynamic)
                {
                    mutable[i] = mutable[i] with { LabelKey = dynamicLeafLabel };
                    break;
                }
            }

            segments = mutable;
        }

        return new BreadcrumbMatch(segments, match.Manifest);
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

    private sealed record RouteEntry(IModuleManifest Manifest, BreadcrumbRoute Route);
}
