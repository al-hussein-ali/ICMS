using ICMS.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config
{
    public class ImmunizationRecordConfig : IEntityTypeConfiguration<ImmunizationRecord>
    {
        public void Configure(EntityTypeBuilder<ImmunizationRecord> builder)
        {
            builder.HasKey(ir => ir.Id);


            builder.Property(ir => ir.Id).HasColumnName("ImmunizationRecordId"); ;


            builder.Property(ir => ir.VaccinationDate).IsRequired();
            builder.Property(ir => ir.TakenIn).HasMaxLength(200).IsUnicode(true).IsRequired();
            builder.Property(ir => ir.Notes).HasMaxLength(500).IsUnicode(true).IsRequired(false);


            builder.HasIndex(ir => ir.IndividualId);
            builder.HasIndex(ir => ir.DoseId);
            builder.HasIndex(ir => ir.FieldVisitId);

            builder.HasOne(ir => ir.VaccinatedIndividual)
                .WithMany(vi => vi.ImmunizationRecords)
                .HasForeignKey(ir => ir.IndividualId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ir => ir.Dose)
                .WithMany()
                .HasForeignKey(ir => ir.DoseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ir => ir.FieldVisit)
                .WithMany(fv => fv.ImmunizationRecords)
                .HasForeignKey(ir => ir.FieldVisitId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
