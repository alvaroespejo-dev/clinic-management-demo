using AEspejo.Clinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AEspejo.Clinic.Application.Interfaces;

/// <summary>
/// Abstraction of the tenant data context for the Application layer.
/// Implemented by <c>AppDbContext</c> in Infrastructure. Lets services
/// use EF Core without depending on the concrete provider (SQL Server).
/// </summary>
public interface IAppDbContext
{
    DbSet<OrgConfig> OrgConfigs { get; }
    DbSet<Branch> Branches { get; }
    DbSet<User> Users { get; }
    DbSet<Professional> Professionals { get; }
    DbSet<Room> Rooms { get; }
    DbSet<Patient> Patients { get; }
    DbSet<PatientAllergy> PatientAllergies { get; }
    DbSet<MedicalCondition> MedicalConditions { get; }
    DbSet<Odontogram> Odontograms { get; }
    DbSet<ToothRecord> ToothRecords { get; }
    DbSet<ServiceCatalog> ServiceCatalogs { get; }
    DbSet<ServiceCatalogTranslation> ServiceCatalogTranslations { get; }
    DbSet<Appointment> Appointments { get; }
    DbSet<AppointmentDetail> AppointmentDetails { get; }
    DbSet<TreatmentPlan> TreatmentPlans { get; }
    DbSet<TreatmentPlanItem> TreatmentPlanItems { get; }
    DbSet<Invoice> Invoices { get; }
    DbSet<Payment> Payments { get; }
    DbSet<AuditLog> AuditLogs { get; }

    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
