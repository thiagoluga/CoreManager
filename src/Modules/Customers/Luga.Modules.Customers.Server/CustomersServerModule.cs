using System.Reflection;

using FluentValidation;

using Luga.BuildingBlocks.Application.Repositories;
using Luga.Modules.Customers.Contracts;
using Luga.Modules.Customers.Server.Application.Repositories;
using Luga.Modules.Customers.Server.Domain.Entities;
using Luga.Modules.Customers.Server.Infrastructure.Persistence;
using Luga.Modules.Customers.Server.Infrastructure.Repositories;
using Luga.Modules.Customers.Server.Infrastructure.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Luga.Modules.Customers.Server;

public static class CustomersServerModule
{
    public static Assembly Assembly => typeof(CustomersServerModule).Assembly;

    public static IServiceCollection AddCustomersServerModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        string connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default is missing.");

        services.AddDbContext<CustomersDbContext>((sp, options) =>
        {
            options.UseSqlServer(
                connectionString,
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", schema: "customers"));

            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });

        services.AddValidatorsFromAssembly(Assembly);

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IRepository<Customer>>(sp => sp.GetRequiredService<ICustomerRepository>());

        services.AddScoped<ICustomersService, CustomersService>();
        return services;
    }
}
