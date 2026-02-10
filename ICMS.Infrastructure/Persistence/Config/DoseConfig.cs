using ICMS.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config
{
    public class DoseConfig : IEntityTypeConfiguration<Dose>
    {
        public void Configure(EntityTypeBuilder<Dose> builder)
        {

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id).HasColumnName("DoseId");

            builder.Property(d => d.DoseName).HasMaxLength(150).IsUnicode(true).IsRequired();
            builder.Property(d => d.DoseOrder).IsRequired();
            builder.Property(d => d.RecommendedAgeGroup).HasMaxLength(100).IsUnicode(true).IsRequired();
            builder.Property(d => d.RecommendedAgeInMonths).IsRequired();
            builder.Property(d => d.Notes).HasMaxLength(500).IsUnicode(true).IsRequired(false);

            builder.HasIndex(d => d.DoseName).IsUnique();
            builder.HasIndex(d => d.VaccineId);

            builder.HasOne(d => d.Vaccine)
                .WithMany(v => v.Doses)
                .HasForeignKey(d => d.VaccineId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
