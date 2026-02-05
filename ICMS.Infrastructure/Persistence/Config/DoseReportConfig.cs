using ICMS.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config
{
    public class DoseReportConfig : IEntityTypeConfiguration<DoseReport>
    {
        public void Configure(EntityTypeBuilder<DoseReport> builder)
        {
            builder.HasKey(dr => dr.Id);

            builder.Property(dr => dr.Id).HasColumnName("DoseReportId");

            builder.Property(dr => dr.Description).HasMaxLength(1000).IsUnicode(true).IsRequired(false);
            builder.Property(dr => dr.CreatedAt).HasDefaultValueSql("TIMEZONE('utc', NOW())").ValueGeneratedOnAdd();

            builder.HasIndex(dr => dr.BatchId);

            builder.HasOne(dr => dr.Batch)
                .WithOne()
                .HasForeignKey<DoseReport>(dr => dr.BatchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(dr => dr.User)
                .WithMany()
                .HasForeignKey(dr => dr.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
