using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Luga.Modules.Personalization.Server.Infrastructure.Persistence;

public sealed class PersonalizationDbContextFactory : IDesignTimeDbContextFactory<PersonalizationDbContext>
{
    public PersonalizationDbContext CreateDbContext(string[] args)
    {
        string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=LugaCore;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;";

        DbContextOptionsBuilder<PersonalizationDbContext> options = new();
        options.UseSqlServer(connectionString, sql =>
            sql.MigrationsHistoryTable("__EFMigrationsHistory", schema: "personalization"));

        return new PersonalizationDbContext(options.Options);
    }
}
