using ICMS.Domain.Entites;
using ICMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config
{
    public  class TransactionConfig : IEntityTypeConfiguration<Transaction>
    {
        private string[] _transactionTypes = { "وارد", "صادر" };

        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasColumnName("TransactionId");

            builder.Property(t => t.TransactionType).HasConversion(
                v => v == TransactionType.In ? _transactionTypes[0] : _transactionTypes[1],
                v => v == _transactionTypes[0] ? TransactionType.In : TransactionType.Out
            ).HasMaxLength(50).IsUnicode(true).IsRequired();

            builder.Property(t => t.TransactionDate).HasDefaultValueSql("TIMEZONE('utc', NOW())").ValueGeneratedOnAdd();
            builder.Property(t => t.PermissionNumber).HasMaxLength(200).IsUnicode(true).IsRequired();
            builder.Property(t => t.SourceorDestination).HasMaxLength(250).IsUnicode(true).IsRequired();
            builder.Property(t => t.Quantity).IsRequired();


            builder.HasIndex(t => t.BatchId);

            builder.HasOne(t => t.Batch)
                .WithMany(b => b.Transactions)
                .HasForeignKey(t => t.BatchId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
