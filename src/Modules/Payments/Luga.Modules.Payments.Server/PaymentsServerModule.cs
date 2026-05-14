using System.Reflection;

using Luga.Modules.Payments.Server.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Luga.Modules.Payments.Server;

/// <summary>
/// Composition root for the Payments module. MVP wires the DbContext + entities;
/// command handlers / controllers / Asaas integration ship in V1.1.
/// </summary>
public static class PaymentsServerModule
{
    public static Assembly Assembly => typeof(PaymentsServerModule).Assembly;

    public static IServiceCollection AddPaymentsServerModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        string connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default is missing.");

        services.AddDbContext<PaymentsDbContext>((sp, options) =>
        {
            options.UseSqlServer(
                connectionString,
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", schema: "payments"));

            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });

        return services;
    }
}
