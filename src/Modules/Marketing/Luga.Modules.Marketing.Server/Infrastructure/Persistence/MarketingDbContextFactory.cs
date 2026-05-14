using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Luga.Modules.Marketing.Server.Infrastructure.Persistence;

public sealed class MarketingDbContextFactory : IDesignTimeDbContextFactory<MarketingDbContext>
{
    public MarketingDbContext CreateDbContext(string[] args)
    {
        string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=LugaCore;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;";

        DbContextOptionsBuilder<MarketingDbContext> options = new();
        options.UseSqlServer(connectionString, sql =>
            sql.MigrationsHistoryTable("__EFMigrationsHistory", schema: "marketing"));

        return new MarketingDbContext(options.Options);
    }
}
