using Luga.BuildingBlocks.Client.Manifests;
using Luga.Modules.Marketing.Shared.Refit;

using Microsoft.AspNetCore.Components;
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
            .ConfigureHttpClient((sp, client) =>
            {
                NavigationManager nav = sp.GetRequiredService<NavigationManager>();
                client.BaseAddress = new Uri(nav.BaseUri);
            });

        return services;
    }
}
