using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config.Clinical
{
    public class BatchConfig : IEntityTypeConfiguration<Batch>
    {
        public void Configure(EntityTypeBuilder<Batch> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id).HasColumnName("BatchId");

            builder.Property<uint>("xmin").HasColumnType("xid").IsRowVersion();

            builder.Property(b => b.BatchName).HasMaxLength(250).IsUnicode(true).IsRequired();
            builder.Property(b => b.CountryOfOrigin).HasMaxLength(150).IsUnicode(true).IsRequired();
            builder.Property(b => b.CookNumber).HasMaxLength(200).IsUnicode(true).IsRequired();
            builder.Property(b => b.ExpiryDate).IsRequired();
            builder.Property(b => b.CreationDate).IsRequired();
            builder.Property(b => b.TotalQuantity).IsRequired();
            builder.Property(b => b.Notes).HasMaxLength(500).IsUnicode(true).IsRequired(false);

            builder.HasIndex(b => new { b.DoseId, b.CountryOfOrigin, b.CookNumber }).IsUnique();
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

            builder.HasQueryFilter(b => !b.User.Person.IsDeleted);
        }
    }
}
