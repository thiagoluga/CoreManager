using Luga.BuildingBlocks.Client.Manifests;
using Luga.Modules.Customers.Shared.Refit;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using Refit;

namespace Luga.Modules.Customers.Client;

public static class CustomersClientModule
{
    public static IServiceCollection AddCustomersClientModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton<IModuleManifest, CustomersManifest>();
        services.AddRefitClient<ICustomersApi>()
            .ConfigureHttpClient((sp, client) =>
            {
                NavigationManager nav = sp.GetRequiredService<NavigationManager>();
                client.BaseAddress = new Uri(nav.BaseUri);
            });
        return services;
    }
}
