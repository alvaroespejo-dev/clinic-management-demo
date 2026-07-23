using AEspejo.Clinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AEspejo.Clinic.Infrastructure.Persistence.Configurations;

public class ServiceCatalogConfiguration : IEntityTypeConfiguration<ServiceCatalog>
{
    public void Configure(EntityTypeBuilder<ServiceCatalog> builder)
    {
        builder.ToTable("ServiceCatalogs");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Code).IsRequired().HasMaxLength(50);
        builder.Property(s => s.DefaultPrice).HasPrecision(18, 2);

        builder.HasIndex(s => s.Code).IsUnique();
    }
}

public class ServiceCatalogTranslationConfiguration : IEntityTypeConfiguration<ServiceCatalogTranslation>
{
    public void Configure(EntityTypeBuilder<ServiceCatalogTranslation> builder)
    {
        builder.ToTable("ServiceCatalogTranslations");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.LanguageCode).IsRequired().HasMaxLength(10);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Description).HasMaxLength(1000);
        builder.Property(t => t.Category).HasMaxLength(150);

        builder.HasOne(t => t.Service)
            .WithMany(s => s.Translations)
            .HasForeignKey(t => t.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => new { t.ServiceId, t.LanguageCode }).IsUnique();
    }
}

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Status).HasConversion<int>();
        builder.Property(a => a.Notes).HasMaxLength(2000);
        builder.Property(a => a.CancelReason).HasMaxLength(500);

        builder.HasIndex(a => a.ScheduledAt);
        builder.HasIndex(a => new { a.BranchId, a.ScheduledAt });

        builder.HasOne(a => a.Branch)
            .WithMany(b => b.Appointments)
            .HasForeignKey(a => a.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Professional)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.ProfessionalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Room)
            .WithMany(r => r.Appointments)
            .HasForeignKey(a => a.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.CreatedByUser)
            .WithMany()
            .HasForeignKey(a => a.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class AppointmentDetailConfiguration : IEntityTypeConfiguration<AppointmentDetail>
{
    public void Configure(EntityTypeBuilder<AppointmentDetail> builder)
    {
        builder.ToTable("AppointmentDetails");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.UnitPrice).HasPrecision(18, 2);
        builder.Property(d => d.Notes).HasMaxLength(1000);

        builder.HasOne(d => d.Appointment)
            .WithMany(a => a.Details)
            .HasForeignKey(d => d.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Service)
            .WithMany()
            .HasForeignKey(d => d.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class TreatmentPlanConfiguration : IEntityTypeConfiguration<TreatmentPlan>
{
    public void Configure(EntityTypeBuilder<TreatmentPlan> builder)
    {
        builder.ToTable("TreatmentPlans");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Description).HasMaxLength(2000);
        builder.Property(t => t.Status).HasConversion<int>();

        builder.HasOne(t => t.Patient)
            .WithMany(p => p.TreatmentPlans)
            .HasForeignKey(t => t.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Professional)
            .WithMany(p => p.TreatmentPlans)
            .HasForeignKey(t => t.ProfessionalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class TreatmentPlanItemConfiguration : IEntityTypeConfiguration<TreatmentPlanItem>
{
    public void Configure(EntityTypeBuilder<TreatmentPlanItem> builder)
    {
        builder.ToTable("TreatmentPlanItems");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.EstimatedPrice).HasPrecision(18, 2);
        builder.Property(i => i.Status).HasConversion<int>();

        builder.HasOne(i => i.TreatmentPlan)
            .WithMany(t => t.Items)
            .HasForeignKey(i => i.TreatmentPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Service)
            .WithMany()
            .HasForeignKey(i => i.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.AppointmentDetail)
            .WithMany()
            .HasForeignKey(i => i.AppointmentDetailId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
