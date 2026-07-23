using AEspejo.Clinic.Master.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AEspejo.Clinic.Master.Configurations;

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable("Languages");
        builder.HasKey(l => l.Code);

        builder.Property(l => l.Code).HasMaxLength(10);
        builder.Property(l => l.Name).IsRequired().HasMaxLength(100);

        // Initial seed: Spanish and English.
        builder.HasData(
            new Language { Code = "es", Name = "Español", IsActive = true },
            new Language { Code = "en", Name = "English", IsActive = true }
        );
    }
}
