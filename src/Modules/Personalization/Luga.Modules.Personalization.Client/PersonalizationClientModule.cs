using Luga.BuildingBlocks.Client.Manifests;

using Microsoft.Extensions.DependencyInjection;

namespace Luga.Modules.Personalization.Client;

/// <summary>DI registration for the Personalization client (manifest only in the MVP).</summary>
public static class PersonalizationClientModule
{
    public static IServiceCollection AddPersonalizationClientModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton<IModuleManifest, PersonalizationManifest>();
        return services;
    }
}
