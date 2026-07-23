using AEspejo.Clinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AEspejo.Clinic.Infrastructure.Persistence.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Name).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Address).HasMaxLength(400);
        builder.Property(b => b.Phone).HasMaxLength(40);
        builder.Property(b => b.Email).HasMaxLength(256);
    }
}

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).IsRequired().HasMaxLength(100);

        builder.HasOne(r => r.Branch)
            .WithMany(b => b.Rooms)
            .HasForeignKey(r => r.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(512);
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Role).HasConversion<int>();
        builder.Property(u => u.PreferredLanguage).IsRequired().HasMaxLength(10);

        builder.HasIndex(u => u.Email).IsUnique();

        builder.HasOne(u => u.Branch)
            .WithMany(b => b.Users)
            .HasForeignKey(u => u.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ProfessionalConfiguration : IEntityTypeConfiguration<Professional>
{
    public void Configure(EntityTypeBuilder<Professional> builder)
    {
        builder.ToTable("Professionals");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.LicenseNumber).HasMaxLength(50);
        builder.Property(p => p.Specialty).HasMaxLength(150);
        builder.Property(p => p.Color).HasMaxLength(20);

        // 1:1 relationship with User; shares the PK.
        builder.HasOne(p => p.User)
            .WithOne(u => u.Professional)
            .HasForeignKey<Professional>(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
