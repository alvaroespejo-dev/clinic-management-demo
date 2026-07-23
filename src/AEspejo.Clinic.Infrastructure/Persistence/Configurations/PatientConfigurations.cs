using AEspejo.Clinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AEspejo.Clinic.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.LastName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Gender).HasConversion<int>();
        builder.Property(p => p.DocumentType).HasConversion<int>();
        builder.Property(p => p.DocumentNumber).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Phone).HasMaxLength(40);
        builder.Property(p => p.Email).HasMaxLength(256);
        builder.Property(p => p.Address).HasMaxLength(400);
        builder.Property(p => p.BloodType).HasMaxLength(10);
        builder.Property(p => p.PreferredLanguage).IsRequired().HasMaxLength(10);
        builder.Property(p => p.EmergencyContactName).HasMaxLength(200);
        builder.Property(p => p.EmergencyContactPhone).HasMaxLength(40);

        builder.HasIndex(p => p.DocumentNumber);

        builder.HasOne(p => p.Branch)
            .WithMany(b => b.Patients)
            .HasForeignKey(p => p.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PatientAllergyConfiguration : IEntityTypeConfiguration<PatientAllergy>
{
    public void Configure(EntityTypeBuilder<PatientAllergy> builder)
    {
        builder.ToTable("PatientAllergies");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Substance).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Severity).HasConversion<int>();

        builder.HasOne(a => a.Patient)
            .WithMany(p => p.Allergies)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class MedicalConditionConfiguration : IEntityTypeConfiguration<MedicalCondition>
{
    public void Configure(EntityTypeBuilder<MedicalCondition> builder)
    {
        builder.ToTable("MedicalConditions");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);

        builder.HasOne(c => c.Patient)
            .WithMany(p => p.MedicalConditions)
            .HasForeignKey(c => c.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class OdontogramConfiguration : IEntityTypeConfiguration<Odontogram>
{
    public void Configure(EntityTypeBuilder<Odontogram> builder)
    {
        builder.ToTable("Odontograms");
        builder.HasKey(o => o.Id);

        // 1:1 with Patient.
        builder.HasOne(o => o.Patient)
            .WithOne(p => p.Odontogram)
            .HasForeignKey<Odontogram>(o => o.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.PatientId).IsUnique();
    }
}

public class ToothRecordConfiguration : IEntityTypeConfiguration<ToothRecord>
{
    public void Configure(EntityTypeBuilder<ToothRecord> builder)
    {
        builder.ToTable("ToothRecords");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Surface).HasConversion<int>();
        builder.Property(t => t.Status).HasConversion<int>();

        builder.HasOne(t => t.Odontogram)
            .WithMany(o => o.ToothRecords)
            .HasForeignKey(t => t.OdontogramId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.UpdatedByUser)
            .WithMany()
            .HasForeignKey(t => t.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
