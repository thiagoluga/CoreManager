using Luga.BuildingBlocks.Client.Manifests;
using Luga.Modules.Marketing.Shared.Refit;

using Microsoft.Extensions.DependencyInjection;

using Refit;

namespace Luga.Modules.Marketing.Client;

/// <summary>DI registration entry point for the Marketing module (CLAUDE.md §6).</summary>
public static class MarketingClientModule
{
    public static IServiceCollection AddMarketingClientModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IModuleManifest, MarketingManifest>();

        services.AddRefitClient<IMarketingApi>()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri("/", UriKind.Relative));

        return services;
    }
}
