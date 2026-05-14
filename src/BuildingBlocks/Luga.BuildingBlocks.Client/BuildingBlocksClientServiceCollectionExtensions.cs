using Luga.BuildingBlocks.Client.Auth;
using Luga.BuildingBlocks.Client.Localization;
using Luga.BuildingBlocks.Client.Navigation;
using Luga.BuildingBlocks.Client.Tenancy;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using MudBlazor.Services;

namespace Luga.BuildingBlocks.Client;

/// <summary>
/// Top-level DI extension that the Blazor WASM host calls once. Registers
/// MudBlazor services, the localization pipeline, the context records and the
/// permission / breadcrumb services. Modules add their own
/// <c>IModuleManifest</c> singletons on top of this baseline.
/// </summary>
public static class BuildingBlocksClientServiceCollectionExtensions
{
    /// <summary>Registers BuildingBlocks.Client services. Idempotent.</summary>
    public static IServiceCollection AddLugaBuildingBlocksClient(
        this IServiceCollection services,
        Action<LocalizationOptions>? configureLocalization = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddMudServices();
        services.AddLugaLocalization(configureLocalization);

        // Per-user state. Hosts replace these instances after authentication completes.
        services.TryAddScoped<TenantContext>(_ => TenantContext.Anonymous);
        services.TryAddScoped<CurrentUser>(_ => CurrentUser.Anonymous);

        services.TryAddScoped<IPermissionService, PermissionService>();
        services.TryAddSingleton<IBreadcrumbResolver, BreadcrumbResolver>();

        return services;
    }
}
