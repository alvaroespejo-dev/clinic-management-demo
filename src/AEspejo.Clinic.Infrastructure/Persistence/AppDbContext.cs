using AEspejo.Clinic.Application.Interfaces;
using AEspejo.Clinic.Domain.Entities;
using AEspejo.Clinic.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace AEspejo.Clinic.Infrastructure.Persistence;

/// <summary>
/// Database context of a TENANT (company). Holds the whole clinical model.
/// The connection string is resolved per request from the active tenant
/// (via <see cref="ITenantConnectionResolver"/>) or injected through DbContextOptions.
/// </summary>
// Single constructor: under DI, EF injects the registered resolver and provider; when instantiated
// manually (design-time factory / provisioning) they stay defaulted and the options already carry the provider.
public class AppDbContext(
    DbContextOptions<AppDbContext> options,
    ITenantConnectionResolver? connectionResolver = null,
    DatabaseProviderAccessor? providerAccessor = null)
    : DbContext(options), IAppDbContext
{
    private readonly ITenantConnectionResolver? _connectionResolver = connectionResolver;
    private readonly DatabaseProvider _provider = providerAccessor?.Provider ?? DatabaseProvider.SqlServer;

    public DbSet<OrgConfig> OrgConfigs => Set<OrgConfig>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Professional> Professionals => Set<Professional>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<PatientAllergy> PatientAllergies => Set<PatientAllergy>();
    public DbSet<MedicalCondition> MedicalConditions => Set<MedicalCondition>();
    public DbSet<Odontogram> Odontograms => Set<Odontogram>();
    public DbSet<ToothRecord> ToothRecords => Set<ToothRecord>();
    public DbSet<ServiceCatalog> ServiceCatalogs => Set<ServiceCatalog>();
    public DbSet<ServiceCatalogTranslation> ServiceCatalogTranslations => Set<ServiceCatalogTranslation>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<AppointmentDetail> AppointmentDetails => Set<AppointmentDetail>();
    public DbSet<TreatmentPlan> TreatmentPlans => Set<TreatmentPlan>();
    public DbSet<TreatmentPlanItem> TreatmentPlanItems => Set<TreatmentPlanItem>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // If not already configured and a tenant resolver is available, use its connection string.
        if (!optionsBuilder.IsConfigured && _connectionResolver is not null)
        {
            var connectionString = _connectionResolver.GetConnectionString();
            if (!string.IsNullOrWhiteSpace(connectionString))
                optionsBuilder.UseConfiguredDatabase(_provider, connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // SQLite needs DateTimeOffset stored as ticks for ORDER BY / comparisons to translate.
        if (Database.IsSqlite())
            DatabaseProviderExtensions.ApplySqliteDateTimeOffsetConverters(modelBuilder);
    }
}
