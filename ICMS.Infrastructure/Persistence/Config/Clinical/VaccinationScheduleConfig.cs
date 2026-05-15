using ICMS.Domain.Entites.Clinical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config.Clinical
{
    public class VaccinationScheduleConfig : IEntityTypeConfiguration<VaccinationSchedule>
    {
        public void Configure(EntityTypeBuilder<VaccinationSchedule> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).HasColumnName("VaccinationScheduleId");

            builder.Property(s => s.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(s => s.ScheduledDate).IsRequired();

            builder.HasOne(s => s.VaccinatedIndividual)
                .WithMany(vi => vi.Schedules)
                .HasForeignKey(s => s.VaccinatedIndividualId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Dose)
                .WithMany()
                .HasForeignKey(s => s.DoseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.ImmunizationRecord)
                .WithOne()
                .HasForeignKey<VaccinationSchedule>(s => s.ImmunizationRecordId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasQueryFilter(s => !s.VaccinatedIndividual.IsDeleted);
        }
    }
}
