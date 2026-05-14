using System.Reflection;

using FluentValidation;

using Luga.BuildingBlocks.Application.Abstractions;
using Luga.BuildingBlocks.Infrastructure.Persistence;
using Luga.BuildingBlocks.Server.Modules;
using Luga.Modules.Core.Contracts;
using Luga.Modules.Core.Server.Application.Repositories;
using Luga.Modules.Core.Server.Infrastructure.Persistence;
using Luga.Modules.Core.Server.Infrastructure.Repositories;
using Luga.Modules.Core.Server.Infrastructure.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Luga.Modules.Core.Server;

/// <summary>
/// Composition root for the Core module. The host calls this once during startup
/// to register every Core-owned service (CLAUDE.md §6).
/// </summary>
public static class CoreServerModule
{
    /// <summary>
    /// Assembly that contains Core's MediatR handlers, FluentValidation validators
    /// and controllers. Exposed so the host can include it in the global MediatR
    /// registration plus the MVC <c>ApplicationPart</c> manager.
    /// </summary>
    public static Assembly Assembly => typeof(CoreServerModule).Assembly;

    /// <summary>
    /// Registers Core module services and DbContext. MediatR + pipeline behaviors
    /// are registered globally by the host (with <see cref="Assembly"/> as one of
    /// the source assemblies) so behaviors run once across every module.
    /// </summary>
    public static IServiceCollection AddCoreServerModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        string connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default is missing.");

        services.AddDbContext<CoreDbContext>((sp, options) =>
        {
            options.UseSqlServer(
                connectionString,
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", schema: "core"));

            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });

        // Bind the cross-module abstractions to CoreDbContext.
        //   - LugaDbContextBase: consumed by `IdempotencyStore` (core.idempotency_keys
        //     is the single, platform-wide table) and by `ProcessedEventStore`.
        //   - IUnitOfWork: consumed by Core's MediatR handlers (RegisterTenantCommandHandler).
        // Other modules' handlers inject their own `XDbContext` directly to commit work
        // against their own schema. CLAUDE.md §7.8 / §16.
        services.AddScoped<LugaDbContextBase>(sp => sp.GetRequiredService<CoreDbContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CoreDbContext>());

        // FluentValidation discovery for Core validators (host registers MediatR globally).
        services.AddValidatorsFromAssembly(Assembly);

        // Repositories
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<ITenantUserRepository, TenantUserRepository>();

        // Cross-module contracts
        services.AddScoped<ITenantsService, TenantsService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<ISubscriptionPlansService, SubscriptionPlansService>();

        // Module initializer (versioned DML seeds — CLAUDE.md §7.11).
        services.AddSingleton<IModuleInitializer, CoreModuleInitializer>();

        return services;
    }
}
