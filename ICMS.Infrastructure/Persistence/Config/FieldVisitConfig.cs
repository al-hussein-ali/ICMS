using ICMS.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config
{
    public class FieldVisitConfig : IEntityTypeConfiguration<FieldVisit>
    {
        public void Configure(EntityTypeBuilder<FieldVisit> builder)
        {
            builder.HasKey(fv => fv.Id);

            builder.Property(fv => fv.Id).HasColumnName("FieldVisitId");

            builder.Property(fv => fv.VisitDate).IsRequired();
            builder.Property(fv => fv.TargetedLocation).HasMaxLength(250).IsUnicode(true).IsRequired();
            builder.Property(fv => fv.IsCompleted).HasDefaultValue(false);


            builder.Navigation(nameof(FieldVisit.ImmunizationRecords))?
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasField("_immunizationRecords");

            builder.Navigation(nameof(FieldVisit.FieldVisitUsers))?
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasField("_fieldVisitUsers");

            builder.HasMany(fv => fv.ImmunizationRecords)
                .WithOne(ir => ir.FieldVisit)
                .HasForeignKey(ir => ir.FieldVisitId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(fv => fv.FieldVisitUsers)
                   .WithOne(fvu => fvu.FieldVisit)
                   .HasForeignKey(fvu => fvu.FieldVisitId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
