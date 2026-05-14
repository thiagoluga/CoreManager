using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace Luga.BuildingBlocks.Client.Localization;

/// <summary>
/// Wires Luga's WASM-compatible JSON localizer (CLAUDE.md §11) with the
/// supported-cultures list and the <see cref="ILugaCultureProvider"/> cascade.
/// </summary>
/// <remarks>
/// We do not use <c>My.Extensions.Localization.Json</c> here because its
/// factory loads JSON via filesystem <c>Path.Combine</c>, which fails inside
/// Blazor WebAssembly (no filesystem in the browser). Our
/// <see cref="LugaJsonStringLocalizer"/> reads the same JSON files as embedded
/// resources, so the resource-files convention from CLAUDE.md §11.4 stays
/// intact and the code works in both WASM and server hosts.
/// </remarks>
public static class LocalizationServiceCollectionExtensions
{
    /// <summary>Registers JSON localization, the supported-cultures list and the culture provider.</summary>
    public static IServiceCollection AddLugaLocalization(
        this IServiceCollection services,
        Action<LocalizationOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        LocalizationOptions options = new();
        configure?.Invoke(options);

        services.TryAddSingleton(options);

        // Register the standard open-generic IStringLocalizer<T> adapter
        // (System.Extensions.Localization.StringLocalizer<T>) which resolves
        // via our IStringLocalizerFactory.
        services.AddLocalization();
        services.Replace(ServiceDescriptor.Singleton<IStringLocalizerFactory, LugaJsonStringLocalizerFactory>());

        services.TryAddSingleton<ILugaCultureProvider, LugaCultureProvider>();

        return services;
    }
}
