using System.Reflection;

using Luga.Modules.Personalization.Server.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Luga.Modules.Personalization.Server;

/// <summary>Composition root for Personalization. Minimal in the MVP.</summary>
public static class PersonalizationServerModule
{
    public static Assembly Assembly => typeof(PersonalizationServerModule).Assembly;

    public static IServiceCollection AddPersonalizationServerModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        string connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default is missing.");

        services.AddDbContext<PersonalizationDbContext>((sp, options) =>
        {
            options.UseSqlServer(
                connectionString,
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", schema: "personalization"));

            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });

        return services;
    }
}
