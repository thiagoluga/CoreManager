using FluentValidation;

using Luga.BuildingBlocks.Application.Behaviors;
using Luga.BuildingBlocks.Server.Modules;
using Luga.Modules.Core.Contracts;
using Luga.Modules.Core.Server.Application.Repositories;
using Luga.Modules.Core.Server.Infrastructure.Persistence;
using Luga.Modules.Core.Server.Infrastructure.Repositories;
using Luga.Modules.Core.Server.Infrastructure.Services;

using MediatR;

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
    /// <summary>Registers Core module services and DbContext.</summary>
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

        // MediatR for this module's handlers + pipeline behaviors registered globally by the host.
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CoreServerModule).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        });

        // FluentValidation discovery for Core validators.
        services.AddValidatorsFromAssembly(typeof(CoreServerModule).Assembly);

        // Repositories
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<ITenantUserRepository, TenantUserRepository>();

        // Cross-module contracts
        services.AddScoped<ITenantsService, TenantsService>();
        services.AddScoped<IUsersService, UsersService>();

        // Module initializer (versioned DML seeds — CLAUDE.md §7.11).
        services.AddSingleton<IModuleInitializer, CoreModuleInitializer>();

        // Controllers are discovered by the host via AddApplicationPart in §5.8.
        return services;
    }
}
