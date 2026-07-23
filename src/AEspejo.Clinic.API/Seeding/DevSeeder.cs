using AEspejo.Clinic.Infrastructure.MultiTenancy;
using AEspejo.Clinic.Infrastructure.Persistence;
using AEspejo.Clinic.Master;
using AEspejo.Clinic.Master.Entities;
using AEspejo.Clinic.Master.Enums;
using Microsoft.EntityFrameworkCore;

namespace AEspejo.Clinic.API.Seeding;

/// <summary>
/// Development seeding: applies the master database migration, ensures a "demo" tenant
/// and provisions its database (tables + branch + admin user).
/// </summary>
public static class DevSeeder
{
    public const string DemoSubdomain = "demo";
    public const string AdminEmail = "admin@demo.local";
    public const string AdminPassword = "Admin12345";

    public static async Task SeedAsync(IServiceProvider services, string masterDbHostConnection)
    {
        using var scope = services.CreateScope();
        var master = scope.ServiceProvider.GetRequiredService<MasterDbContext>();
        var provisioning = scope.ServiceProvider.GetRequiredService<TenantProvisioningService>();
        var provider = scope.ServiceProvider.GetRequiredService<DatabaseProviderAccessor>().Provider;

        // 1) Master database up to date (migrations on SQL Server, EnsureCreated on SQLite).
        await master.EnsureDatabaseAsync(provider);

        // 2) Demo tenant.
        var demo = await master.Tenants.FirstOrDefaultAsync(t => t.Subdomain == DemoSubdomain);
        if (demo is null)
        {
            demo = new Tenant
            {
                CompanyName = "Clínica Demo",
                Subdomain = DemoSubdomain,
                DatabaseName = "AEspejo_Clinic_Demo",
                ConnectionString = BuildTenantConnection(masterDbHostConnection, "AEspejo_Clinic_Demo", provider),
                Plan = TenantPlan.Trial,
                DefaultLanguage = "es",
                ContactEmail = AdminEmail,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow
            };
            master.Tenants.Add(demo);
            await master.SaveChangesAsync();
        }

        // 3) Provision the demo tenant database (create/migrate + admin + branch).
        await provisioning.ProvisionAsync(demo, AdminEmail, AdminPassword);
    }

    /// <summary>
    /// Derives the tenant connection string. On SQL Server it clones the master connection and swaps the
    /// database name; on SQLite it points to a per-tenant file under the data directory.
    /// </summary>
    private static string BuildTenantConnection(string masterConnection, string tenantDb, DatabaseProvider provider)
    {
        if (provider == DatabaseProvider.Sqlite)
            return DatabaseProviderExtensions.BuildSqliteConnectionString(tenantDb);

        var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(masterConnection)
        {
            InitialCatalog = tenantDb
        };
        return builder.ConnectionString;
    }
}
