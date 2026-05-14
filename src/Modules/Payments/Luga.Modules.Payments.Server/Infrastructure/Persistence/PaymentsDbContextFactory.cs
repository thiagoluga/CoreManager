using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Luga.Modules.Payments.Server.Infrastructure.Persistence;

public sealed class PaymentsDbContextFactory : IDesignTimeDbContextFactory<PaymentsDbContext>
{
    public PaymentsDbContext CreateDbContext(string[] args)
    {
        string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=LugaCore;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;";

        DbContextOptionsBuilder<PaymentsDbContext> options = new();
        options.UseSqlServer(connectionString, sql =>
            sql.MigrationsHistoryTable("__EFMigrationsHistory", schema: "payments"));

        return new PaymentsDbContext(options.Options);
    }
}
