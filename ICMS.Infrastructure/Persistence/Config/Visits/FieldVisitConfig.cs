using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config.Visits
{
    public class FieldVisitConfig : IEntityTypeConfiguration<FieldVisit>
    {
        public void Configure(EntityTypeBuilder<FieldVisit> builder)
        {
            builder.HasKey(fv => fv.Id);

            builder.Property(fv => fv.Id).HasColumnName("FieldVisitId");

            builder.Property(fv => fv.CampaignName).IsRequired().HasMaxLength(250);
            builder.Property(fv => fv.VisitDate).IsRequired();
            builder.HasOne(fv => fv.SubNeighborhood)
                   .WithMany()
                   .HasForeignKey(fv => fv.SubNeighborhoodId)
                   .OnDelete(DeleteBehavior.Restrict);
                   
            builder.Property(fv => fv.IsCompleted).HasDefaultValue(false);

            builder.Property(fv => fv.FromDate).IsRequired();
            builder.Property(fv => fv.ToDate).IsRequired();


            builder.Navigation(nameof(FieldVisit.ImmunizationRecords))?
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasField("_immunizationRecords");

            builder.Navigation(nameof(FieldVisit.FieldVisitIndividuals))?
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasField("_fieldVisitIndividuals");

            builder.Navigation(nameof(FieldVisit.FieldVisitWorkers))?
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasField("_fieldVisitWorkers");


            builder.HasMany(fv => fv.ImmunizationRecords)
                .WithOne(ir => ir.FieldVisit)
                .HasForeignKey(ir => ir.FieldVisitId)
                .OnDelete(DeleteBehavior.Restrict);


    
        }
    }
}
