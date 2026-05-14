using System.Reflection;

using FluentValidation;

using Luga.Modules.Marketing.Server.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Luga.Modules.Marketing.Server;

/// <summary>
/// Composition root for the Marketing module.
/// </summary>
public static class MarketingServerModule
{
    public static Assembly Assembly => typeof(MarketingServerModule).Assembly;

    public static IServiceCollection AddMarketingServerModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        string connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default is missing.");

        services.AddDbContext<MarketingDbContext>((sp, options) =>
        {
            options.UseSqlServer(
                connectionString,
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", schema: "marketing"));

            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });

        services.AddValidatorsFromAssembly(Assembly);

        // No module initializer yet — Marketing has no seeded data in the MVP.
        return services;
    }
}
