using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AEspejo.Clinic.Infrastructure.Persistence;

/// <summary>
/// Design-time factory so the EF CLI (`dotnet ef`) can create migrations
/// for a tenant model. The generated migration is then applied to EVERY company database.
/// The connection string is used at design time only.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var provider = DatabaseProviderExtensions.Parse(Environment.GetEnvironmentVariable("EF_PROVIDER"));

        var connectionString = Environment.GetEnvironmentVariable("TENANT_CONNECTION")
            ?? (provider == DatabaseProvider.Sqlite
                ? DatabaseProviderExtensions.BuildSqliteConnectionString("AEspejo_Clinic_TenantTemplate")
                : "Server=(localdb)\\mssqllocaldb;Database=AEspejo_Clinic_TenantTemplate;Trusted_Connection=True;TrustServerCertificate=True;");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseConfiguredDatabase(provider, connectionString)
            .Options;

        return new AppDbContext(options);
    }
}
