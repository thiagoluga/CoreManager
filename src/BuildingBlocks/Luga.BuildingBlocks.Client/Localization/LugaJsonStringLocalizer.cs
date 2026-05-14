using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Text.Json;

using Microsoft.Extensions.Localization;

namespace Luga.BuildingBlocks.Client.Localization;

/// <summary>
/// JSON-backed <see cref="IStringLocalizer"/> that reads translations from
/// embedded resources (<c>&lt;EmbeddedResource Include="Resources\**\*.json" /&gt;</c>)
/// inside the resource type's assembly.
/// </summary>
/// <remarks>
/// <para>
/// Built for Blazor WebAssembly: <c>My.Extensions.Localization.Json</c> (CLAUDE.md
/// §11.2) cannot run in the browser because it loads JSON via filesystem
/// <c>Path.Combine</c>. This implementation reads via
/// <see cref="Assembly.GetManifestResourceStream(string)"/>, which works in both
/// WASM and server hosts.
/// </para>
/// <para>
/// Resource convention: <c>{AssemblyName}.Resources.{TypeName}.{Culture}.json</c>.
/// Lookup walks the culture parent chain and finally falls back to
/// <see cref="LocalizationOptions.FallbackCulture"/> (pt-BR by default,
/// CLAUDE.md §11.3 / §11.7).
/// </para>
/// </remarks>
public sealed class LugaJsonStringLocalizer : IStringLocalizer
{
    private static readonly IReadOnlyDictionary<string, string> EmptyDictionary =
        new Dictionary<string, string>(0);

    private readonly Type _resourceSource;
    private readonly LocalizationOptions _options;
    private readonly ConcurrentDictionary<string, IReadOnlyDictionary<string, string>> _byCulture =
        new(StringComparer.Ordinal);

    /// <summary>Creates a localizer for <paramref name="resourceSource"/>.</summary>
    public LugaJsonStringLocalizer(Type resourceSource, LocalizationOptions options)
    {
        ArgumentNullException.ThrowIfNull(resourceSource);
        ArgumentNullException.ThrowIfNull(options);

        _resourceSource = resourceSource;
        _options = options;
    }

    /// <inheritdoc />
    public LocalizedString this[string name]
    {
        get
        {
            ArgumentException.ThrowIfNullOrEmpty(name);

            string? value = Lookup(name, CultureInfo.CurrentUICulture);
            return value is null
                ? new LocalizedString(name, name, resourceNotFound: true)
                : new LocalizedString(name, value, resourceNotFound: false);
        }
    }

    /// <inheritdoc />
    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            ArgumentException.ThrowIfNullOrEmpty(name);

            LocalizedString localized = this[name];
            if (localized.ResourceNotFound)
            {
                return localized;
            }

            string formatted = string.Format(
                CultureInfo.CurrentCulture,
                localized.Value,
                arguments);
            return new LocalizedString(name, formatted, resourceNotFound: false);
        }
    }

    /// <inheritdoc />
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        IReadOnlyDictionary<string, string> dict = LoadForCulture(CultureInfo.CurrentUICulture)
            ?? EmptyDictionary;

        foreach (KeyValuePair<string, string> kv in dict)
        {
            yield return new LocalizedString(kv.Key, kv.Value, resourceNotFound: false);
        }
    }

    private static Dictionary<string, string> ParseJson(Stream stream)
    {
        using JsonDocument document = JsonDocument.Parse(stream);
        Dictionary<string, string> dict = new(StringComparer.Ordinal);

        if (document.RootElement.ValueKind != JsonValueKind.Object)
        {
            return dict;
        }

        foreach (JsonProperty property in document.RootElement.EnumerateObject())
        {
            if (property.Value.ValueKind == JsonValueKind.String)
            {
                dict[property.Name] = property.Value.GetString() ?? string.Empty;
            }
        }

        return dict;
    }

    private string? Lookup(string name, CultureInfo culture)
    {
        foreach (CultureInfo c in BuildCultureChain(culture))
        {
            IReadOnlyDictionary<string, string>? dict = LoadForCulture(c);
            if (dict is not null && dict.TryGetValue(name, out string? value))
            {
                return value;
            }
        }

        return null;
    }

    private IReadOnlyDictionary<string, string>? LoadForCulture(CultureInfo culture)
    {
        string cacheKey = culture.Name;
        if (_byCulture.TryGetValue(cacheKey, out IReadOnlyDictionary<string, string>? cached))
        {
            return cached.Count == 0 ? null : cached;
        }

        Assembly assembly = _resourceSource.Assembly;
        string assemblyName = assembly.GetName().Name
            ?? throw new InvalidOperationException(
                $"Assembly for '{_resourceSource.FullName}' has no name.");

        string resourceName =
            $"{assemblyName}.{_options.ResourcesPath}.{_resourceSource.Name}.{culture.Name}.json";

        using Stream? stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            // Cache the miss as an empty dict so we don't hit the assembly again
            // for the same (type, culture) pair.
            _byCulture[cacheKey] = EmptyDictionary;
            return null;
        }

        Dictionary<string, string> parsed = ParseJson(stream);
        _byCulture[cacheKey] = parsed;
        return parsed;
    }

    private IEnumerable<CultureInfo> BuildCultureChain(CultureInfo culture)
    {
        HashSet<string> seen = new(StringComparer.Ordinal);

        CultureInfo current = culture;
        while (!Equals(current, CultureInfo.InvariantCulture))
        {
            if (seen.Add(current.Name))
            {
                yield return current;
            }

            current = current.Parent;
        }

        if (seen.Add(_options.FallbackCulture.Name))
        {
            yield return _options.FallbackCulture;
        }
    }
}
