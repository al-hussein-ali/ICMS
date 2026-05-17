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
    public class VisitDetailsConfig : IEntityTypeConfiguration<VisitDetails>
    {
        public void Configure(EntityTypeBuilder<VisitDetails> builder)
        {
            builder.HasKey(vd => vd.Id);
            builder.Property(vd => vd.Id).HasColumnName("VisitDetailsId");

            builder.Property(vd => vd.VisitDate).HasDefaultValueSql("(NOW() AT TIME ZONE 'utc')::date").IsRequired();
            builder.Property(vd => vd.NextVisitDate).IsRequired(false);

            builder.Property(vd => vd.ClinicalExaminationAndObservation).HasMaxLength(1000).IsUnicode(true).IsRequired(false);

            builder.Property(vd => vd.WeightInKilo).HasPrecision(8,2).IsRequired();


            builder.Property(vd => vd.BloodPressure).HasMaxLength(30).IsUnicode(true).IsRequired();

            builder.Property(vd => vd.APPInUrineTest).HasMaxLength(30).IsUnicode(true).IsRequired();

            builder.Property(vd => vd.OGTTInUrineTest).HasMaxLength(30).IsUnicode(true).IsRequired();

            builder.Property(vd => vd.FetalHeartbeat).HasMaxLength(30).IsUnicode(true).IsRequired();

            builder.Property(vd => vd.FetalMovement).HasMaxLength(50).IsUnicode(true).IsRequired();

            builder.Property(vd => vd.FetalPosition).HasMaxLength(40).IsUnicode(true).IsRequired();

            builder.Property(vd => vd.PregnancyDurationInWeeks).IsRequired();
            builder.Property(vd => vd.AnaemiaOrHemoglobinType).HasMaxLength(100).IsUnicode(true).IsRequired();
            builder.Property(vd => vd.LegsSwelling).HasDefaultValue(false);
            builder.Property(vd => vd.VaginalBleeding).HasDefaultValue(false);

            builder.HasIndex(vd => vd.PregnancyDetailsId);

            builder.HasOne(vd => vd.PregnancyDetails)
                .WithMany(pd => pd.VisitDetails)
                .HasForeignKey(vd => vd.PregnancyDetailsId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(vd => vd.User)
                .WithMany()
                .HasForeignKey(vd => vd.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(vd => !vd.User.Person.IsDeleted && !vd.PregnancyDetails.PregnantWoman.Person.IsDeleted);
        }
    }
}
