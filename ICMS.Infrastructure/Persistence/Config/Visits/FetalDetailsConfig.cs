using ICMS.Domain.Entites.Visits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config.Visits
{
    public class FetalDetailsConfig : IEntityTypeConfiguration<FetalDetails>
    {
        public void Configure(EntityTypeBuilder<FetalDetails> builder)
        {
            builder.HasKey(fd => fd.Id);
            builder.Property(fd => fd.Id).HasColumnName("FetalDetailId");

            builder.Property(fd => fd.FetusLabel).HasMaxLength(50).IsUnicode(true).IsRequired();
            builder.Property(fd => fd.FetalHeartbeat).HasMaxLength(30).IsUnicode(true).IsRequired();
            builder.Property(fd => fd.FetalHeartbeatValue).HasMaxLength(30).IsUnicode(true).IsRequired(false);
            builder.Property(fd => fd.FetalMovement).HasMaxLength(50).IsUnicode(true).IsRequired();
            builder.Property(fd => fd.FetalPosition).HasMaxLength(40).IsUnicode(true).IsRequired();

            builder.HasOne(fd => fd.VisitDetails)
                .WithMany(vd => vd.FetalDetailsList)
                .HasForeignKey(fd => fd.VisitDetailsId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
