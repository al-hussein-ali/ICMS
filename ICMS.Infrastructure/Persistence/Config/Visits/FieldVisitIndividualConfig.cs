using ICMS.Domain.Entites.Visits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config.Visits
{
    public class FieldVisitIndividualConfig : IEntityTypeConfiguration<FieldVisitIndividual>
    {
        public void Configure(EntityTypeBuilder<FieldVisitIndividual> builder)
        {
            builder.ToTable("FieldVisitIndividuals");
            
            builder.HasKey(fvi => new { fvi.FieldVisitId, fvi.VaccinatedIndividualId });

            builder.HasOne(fvi => fvi.FieldVisit)
                .WithMany(fv => fv.FieldVisitIndividuals)
                .HasForeignKey(fvi => fvi.FieldVisitId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(fvi => fvi.VaccinatedIndividual)
                .WithMany()
                .HasForeignKey(fvi => fvi.VaccinatedIndividualId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
