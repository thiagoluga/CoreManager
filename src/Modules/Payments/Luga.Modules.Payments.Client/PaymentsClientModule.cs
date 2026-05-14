using Luga.BuildingBlocks.Client.Manifests;

using Microsoft.Extensions.DependencyInjection;

namespace Luga.Modules.Payments.Client;

public static class PaymentsClientModule
{
    public static IServiceCollection AddPaymentsClientModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton<IModuleManifest, PaymentsManifest>();
        return services;
    }
}
