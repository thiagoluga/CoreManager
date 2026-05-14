#nullable enable

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Luga.BuildingBlocks.Infrastructure.Persistence.Migrations;

/// <summary>
/// Ensures the target SQL Server database exists before any service tries to connect.
/// Required because some clients (e.g. Hangfire's <c>SqlServerStorage</c>) attempt to
/// open a connection during DI/host build, well before EF Core's
/// <c>Database.MigrateAsync</c> would create the database for us.
/// </summary>
/// <remarks>
/// Used in development startup (<c>ApplyMigrationsOnStartup=true</c>). Production
/// provisions the database via infrastructure-as-code, so this helper is a no-op
/// in those environments.
/// </remarks>
public static class DatabaseBootstrapper
{
    /// <summary>
    /// Connects to the <c>master</c> database on the same server and executes
    /// <c>IF NOT EXISTS CREATE DATABASE</c> for the database named in
    /// <paramref name="connectionString"/>. Returns silently when the database
    /// already exists.
    /// </summary>
    public static async Task EnsureDatabaseExistsAsync(
        string connectionString,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        SqlConnectionStringBuilder originalBuilder = new(connectionString);
        string databaseName = originalBuilder.InitialCatalog;

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new InvalidOperationException(
                "Connection string is missing 'Initial Catalog' / 'Database'.");
        }

        // Switch the catalog to 'master' so we can issue CREATE DATABASE.
        SqlConnectionStringBuilder masterBuilder = new(connectionString)
        {
            InitialCatalog = "master",
        };

        await using SqlConnection connection = new(masterBuilder.ConnectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        // Database identifier is bracket-quoted to handle names with hyphens or
        // other identifier-sensitive characters. We intentionally interpolate
        // because T-SQL DDL does not accept the name via a parameter.
        string sql = $"IF DB_ID(@name) IS NULL EXEC('CREATE DATABASE [{databaseName.Replace("]", "]]", StringComparison.Ordinal)}]');";

        await using SqlCommand command = new(sql, connection);
        command.Parameters.Add(new SqlParameter("@name", databaseName));

        int affected = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

        logger?.LogInformation(
            "Ensured SQL Server database '{Database}' exists on '{Server}'.",
            databaseName,
            originalBuilder.DataSource);

        _ = affected;
    }
}
