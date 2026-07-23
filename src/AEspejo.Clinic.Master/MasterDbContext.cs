using AEspejo.Clinic.Master.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AEspejo.Clinic.Master;

/// <summary>
/// MASTER database context: catalog of companies (tenants) and languages.
/// Always connects to the master database; independent of any tenant.
/// </summary>
public class MasterDbContext(DbContextOptions<MasterDbContext> options) : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Language> Languages => Set<Language>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MasterDbContext).Assembly);

        // SQLite needs DateTimeOffset stored as ticks for ORDER BY / comparisons to translate.
        // (Kept local: this project can't reference the Infrastructure helper.)
        if (Database.IsSqlite())
            ApplySqliteDateTimeOffsetConverters(modelBuilder);
    }

    private static void ApplySqliteDateTimeOffsetConverters(ModelBuilder modelBuilder)
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
