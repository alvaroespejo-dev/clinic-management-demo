using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AEspejo.Clinic.Master;

/// <summary>
/// Design-time factory so the EF CLI (`dotnet ef`) can create/apply
/// master database migrations without starting the API. The connection string is used at design time only.
/// </summary>
public class MasterDbContextFactory : IDesignTimeDbContextFactory<MasterDbContext>
{
    public MasterDbContext CreateDbContext(string[] args)
    {
        // Local provider check (this project can't reference Infrastructure's DatabaseProvider helper).
        var useSqlite = string.Equals(
            Environment.GetEnvironmentVariable("EF_PROVIDER"), "Sqlite", StringComparison.OrdinalIgnoreCase);

        var connectionString = Environment.GetEnvironmentVariable("MASTER_CONNECTION")
            ?? (useSqlite
                ? "Data Source=AEspejo_Clinic_Master.db"
                : "Server=(localdb)\\mssqllocaldb;Database=AEspejo_Clinic_Master;Trusted_Connection=True;TrustServerCertificate=True;");

        var builder = new DbContextOptionsBuilder<MasterDbContext>();
        if (useSqlite)
            builder.UseSqlite(connectionString);
        else
            builder.UseSqlServer(connectionString);

        return new MasterDbContext(builder.Options);
    }
}
