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
    public class FieldVisitUserConfig : IEntityTypeConfiguration<FieldVisitUser>
    {
        public void Configure(EntityTypeBuilder<FieldVisitUser> builder)
        {
            builder.HasKey(fvu => new { fvu.FieldVisitId, fvu.UserId });


            builder.HasOne(fvu => fvu.FieldVisit)
                .WithMany(fv => fv.FieldVisitUsers)
                .HasForeignKey(fvu => fvu.FieldVisitId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(fvu => fvu.FieldWorker)
                .WithMany(u => u.FieldVisitUsers)
                .HasForeignKey(fvu => fvu.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
