using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Luga.BuildingBlocks.Client.Localization;

/// <summary>
/// Wires <c>My.Extensions.Localization.Json</c> with the Luga conventions
/// (resources under <c>Resources/</c>, the supported-cultures set, and the
/// <see cref="ILugaCultureProvider"/> cascade).
/// </summary>
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
        services.AddJsonLocalization(o => o.ResourcesPath = options.ResourcesPath);
        services.TryAddSingleton<ILugaCultureProvider, LugaCultureProvider>();

        return services;
    }
}
