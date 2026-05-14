using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Luga.Modules.Customers.Server.Infrastructure.Persistence;

public sealed class CustomersDbContextFactory : IDesignTimeDbContextFactory<CustomersDbContext>
{
    public CustomersDbContext CreateDbContext(string[] args)
    {
        string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=LugaCore;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;";

        DbContextOptionsBuilder<CustomersDbContext> options = new();
        options.UseSqlServer(connectionString, sql =>
            sql.MigrationsHistoryTable("__EFMigrationsHistory", schema: "customers"));

        return new CustomersDbContext(options.Options);
    }
}
