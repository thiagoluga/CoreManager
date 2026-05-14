using Luga.BuildingBlocks.Client.Manifests;
using Luga.Modules.Core.Shared.Refit;

using Microsoft.Extensions.DependencyInjection;

using Refit;

namespace Luga.Modules.Core.Client;

/// <summary>
/// DI registration entry point invoked once by the Blazor WASM host
/// (CLAUDE.md §6 — convention <c>X.ClientModule.AddXClientModule</c>).
/// </summary>
public static class CoreClientModule
{
    /// <summary>Registers the Core manifest and the Refit client.</summary>
    public static IServiceCollection AddCoreClientModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IModuleManifest, CoreManifest>();

        services.AddRefitClient<ICoreApi>()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri("/", UriKind.Relative));

        return services;
    }
}
