using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AEspejo.Clinic.Infrastructure.Persistence;

/// <summary>Supported database engines. Selected via configuration (<c>Database:Provider</c>).</summary>
public enum DatabaseProvider
{
    SqlServer,
    Sqlite
}

/// <summary>
/// Reference-type carrier of the app-global provider, so it can be registered in DI and injected
/// into <c>AppDbContext</c>'s constructor (value types can't be resolved as services).
/// </summary>
public sealed class DatabaseProviderAccessor(DatabaseProvider provider)
{
    public DatabaseProvider Provider { get; } = provider;
}

/// <summary>
/// Single place that knows about concrete EF Core providers. Everything else selects an engine
/// through <see cref="DatabaseProvider"/> and calls these helpers, so adding/replacing a provider
/// only touches this file.
/// </summary>
public static class DatabaseProviderExtensions
{
    /// <summary>Parses the configured provider name; defaults to SQL Server when missing/unknown.</summary>
    public static DatabaseProvider Parse(string? value) =>
        string.Equals(value, "Sqlite", StringComparison.OrdinalIgnoreCase)
            ? DatabaseProvider.Sqlite
            : DatabaseProvider.SqlServer;

    /// <summary>Configures the options builder for the given provider and connection string.</summary>
    public static DbContextOptionsBuilder UseConfiguredDatabase(
        this DbContextOptionsBuilder options, DatabaseProvider provider, string connectionString)
    {
        if (provider == DatabaseProvider.Sqlite)
        {
            // SQLite creates the .db file but not its parent folder; make sure the folder exists.
            EnsureSqliteFileDirectory(connectionString);
            return options.UseSqlite(connectionString);
        }

        return options.UseSqlServer(connectionString);
    }

    /// <summary>Creates the directory that will hold the SQLite database file, if it doesn't exist yet.</summary>
    private static void EnsureSqliteFileDirectory(string connectionString)
    {
        var dataSource = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(connectionString).DataSource;
        if (string.IsNullOrWhiteSpace(dataSource) || dataSource == ":memory:")
            return;

        var dir = Path.GetDirectoryName(Path.GetFullPath(dataSource));
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
    }

    /// <summary>Generic overload that preserves the <c>DbContextOptionsBuilder&lt;TContext&gt;</c> type.</summary>
    public static DbContextOptionsBuilder<TContext> UseConfiguredDatabase<TContext>(
        this DbContextOptionsBuilder<TContext> options, DatabaseProvider provider, string connectionString)
        where TContext : DbContext
    {
        UseConfiguredDatabase((DbContextOptionsBuilder)options, provider, connectionString);
        return options;
    }

    /// <summary>
    /// Brings a database to a usable schema: migrations on SQL Server (versioned, production),
    /// <c>EnsureCreated</c> on SQLite (disposable demo schema built from the current model).
    /// </summary>
    public static async Task EnsureDatabaseAsync(
        this DbContext context, DatabaseProvider provider, CancellationToken ct = default)
    {
        if (provider == DatabaseProvider.Sqlite)
            await context.Database.EnsureCreatedAsync(ct);
        else
            await context.Database.MigrateAsync(ct);
    }

    /// <summary>
    /// Directory that holds SQLite files. Priority: an explicit <c>SQLITE_DATA_DIR</c> override (used by the
    /// local <c>sqlite</c> launch profile), else Azure App Service's persistent <c>%HOME%\data</c>, else the
    /// app's <c>./data</c>. The directory is created if missing.
    /// </summary>
    public static string ResolveDataDirectory()
    {
        var explicitDir = Environment.GetEnvironmentVariable("SQLITE_DATA_DIR");
        var home = Environment.GetEnvironmentVariable("HOME");

        var baseDir = !string.IsNullOrWhiteSpace(explicitDir)
            ? explicitDir
            : !string.IsNullOrWhiteSpace(home)
                ? Path.Combine(home, "data")
                : Path.Combine(AppContext.BaseDirectory, "data");

        Directory.CreateDirectory(baseDir);
        return baseDir;
    }

    /// <summary>Builds a SQLite connection string for a database file named after the tenant DB.</summary>
    public static string BuildSqliteConnectionString(string databaseName)
    {
        var file = Path.Combine(ResolveDataDirectory(), $"{databaseName}.db");
        return $"Data Source={file}";
    }

    /// <summary>
    /// SQLite can't ORDER BY / compare <see cref="DateTimeOffset"/> natively. Store those columns as UTC
    /// tick counts (long) so ordering and range filters translate. Apply only under SQLite from
    /// <c>OnModelCreating</c>; SQL Server keeps its native <c>datetimeoffset</c>. The app uses UTC
    /// throughout, so normalizing to a zero offset is lossless here.
    /// </summary>
    public static void ApplySqliteDateTimeOffsetConverters(ModelBuilder modelBuilder)
    {
        var toTicks = new ValueConverter<DateTimeOffset, long>(
            v => v.UtcTicks,
            v => new DateTimeOffset(v, TimeSpan.Zero));
        var toTicksNullable = new ValueConverter<DateTimeOffset?, long?>(
            v => v.HasValue ? v.Value.UtcTicks : null,
            v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTimeOffset))
                    property.SetValueConverter(toTicks);
                else if (property.ClrType == typeof(DateTimeOffset?))
                    property.SetValueConverter(toTicksNullable);
            }
        }
    }
}
