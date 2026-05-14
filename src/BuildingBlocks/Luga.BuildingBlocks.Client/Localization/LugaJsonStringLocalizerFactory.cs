using System.Collections.Concurrent;

using Microsoft.Extensions.Localization;

namespace Luga.BuildingBlocks.Client.Localization;

/// <summary>
/// Factory that produces <see cref="LugaJsonStringLocalizer"/> instances and
/// caches one per resource type. Caching is safe because the localizer is
/// stateless beyond its own per-culture dictionary cache.
/// </summary>
public sealed class LugaJsonStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly LocalizationOptions _options;
    private readonly ConcurrentDictionary<Type, IStringLocalizer> _byType = new();

    /// <summary>Creates the factory.</summary>
    public LugaJsonStringLocalizerFactory(LocalizationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options;
    }

    /// <inheritdoc />
    public IStringLocalizer Create(Type resourceSource)
    {
        ArgumentNullException.ThrowIfNull(resourceSource);
        return _byType.GetOrAdd(
            resourceSource,
            static (type, opts) => new LugaJsonStringLocalizer(type, opts),
            _options);
    }

    /// <summary>
    /// Not supported. <see cref="StringLocalizer{T}"/> always resolves through
    /// <see cref="Create(Type)"/>; the string overload would require trim-hostile
    /// reflection (<c>Assembly.GetType(string)</c>) and we have no caller for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Always.</exception>
    public IStringLocalizer Create(string baseName, string location) =>
        throw new NotSupportedException(
            "LugaJsonStringLocalizerFactory only supports the IStringLocalizerFactory.Create(Type) overload. "
            + $"Inject IStringLocalizer<{nameof(Type)}> or call factory.Create(typeof(YourType)).");
}
