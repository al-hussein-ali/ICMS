using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config.Clinical
{
    public class VaccineConfig : IEntityTypeConfiguration<Vaccine>
    {
        public void Configure(EntityTypeBuilder<Vaccine> builder)
        {
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Id).HasColumnName("VaccineId");

            builder.Property(v => v.VaccineName).HasMaxLength(100).IsUnicode(true).IsRequired();
            builder.Property(v => v.VaccineCode).HasMaxLength(100).IsUnicode(true).IsRequired();
            builder.Property(v => v.Description).HasMaxLength(600).IsUnicode(true).IsRequired(false);
            builder.Property(v => v.IsActive).HasDefaultValue(true);
            builder.Property(v => v.TotalDosages).IsRequired();
            builder.Property(v => v.MinEligibleAgeInMonths).IsRequired();
            builder.Property(v => v.MaxEligibleAgeInMonths).IsRequired();
            builder.Property(v => v.Audience).IsRequired();

            builder.HasIndex(v => v.VaccineCode).IsUnique();

            builder.HasMany(v => v.Doses)
                .WithOne(d => d.Vaccine)
                .HasForeignKey(d => d.VaccineId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(nameof(Vaccine.Doses))?
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasField("_doses");
        }
    }
}
