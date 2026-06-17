using ICMS.Domain.Entites.Visits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config.Visits
{
    public class FieldVisitWorkerConfig : IEntityTypeConfiguration<FieldVisitWorker>
    {
        public void Configure(EntityTypeBuilder<FieldVisitWorker> builder)
        {
            builder.ToTable("FieldVisitWorkers");
            
            builder.HasKey(fvw => new { fvw.FieldVisitId, fvw.UserId });

            builder.Property(fvw => fvw.IsGoing)
                .HasDefaultValue(true);

            builder.HasOne(fvw => fvw.FieldVisit)
                .WithMany(fv => fv.FieldVisitWorkers)
                .HasForeignKey(fvw => fvw.FieldVisitId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(fvw => fvw.User)
                .WithMany()
                .HasForeignKey(fvw => fvw.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
