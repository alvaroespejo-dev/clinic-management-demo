using AEspejo.Clinic.Master.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AEspejo.Clinic.Master.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.CompanyName).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Subdomain).IsRequired().HasMaxLength(63);
        builder.Property(t => t.DatabaseName).IsRequired().HasMaxLength(128);
        builder.Property(t => t.ConnectionString).IsRequired().HasMaxLength(1024);
        builder.Property(t => t.Plan).HasConversion<int>();
        builder.Property(t => t.DefaultLanguage).IsRequired().HasMaxLength(10);
        builder.Property(t => t.ContactEmail).IsRequired().HasMaxLength(256);

        builder.HasIndex(t => t.Subdomain).IsUnique();
    }
}
