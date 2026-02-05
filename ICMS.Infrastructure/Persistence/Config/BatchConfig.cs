using ICMS.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config
{
    public class BatchConfig : IEntityTypeConfiguration<Batch>
    {
        public void Configure(EntityTypeBuilder<Batch> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id).HasColumnName("BatchId");

            builder.Property(b => b.CountryOfOrigin).HasMaxLength(150).IsUnicode(true).IsRequired();
            builder.Property(b => b.CookNumber).HasMaxLength(200).IsUnicode(true).IsRequired(false);
            builder.Property(b => b.ExpiryDate).IsRequired();
            builder.Property(b => b.TotalQuantity).IsRequired();
            builder.Property(b => b.Notes).HasMaxLength(500).IsUnicode(true).IsRequired(false);

            builder.HasIndex(b => b.DoseId);

            builder.HasOne(b => b.Dose)
                .WithMany()
                .HasForeignKey(b => b.DoseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(nameof(Batch.Transactions))?
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasField("_transactions");
        }
    }
}
