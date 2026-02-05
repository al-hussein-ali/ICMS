using ICMS.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config
{
    public class PreviousPregnancyDeliveryComplicationsConfig : IEntityTypeConfiguration<PreviousPregnancyDeliveryComplications>
    {
        public void Configure(EntityTypeBuilder<PreviousPregnancyDeliveryComplications> builder)
        {
            builder.HasKey(ppd => ppd.Id);
            builder.Property(ppd => ppd.Id).HasColumnName("PreviousPregnancyDeliveryComplicationsId");

            builder.Property(ppd => ppd.CesareanSection).HasDefaultValue(false);
            builder.Property(ppd => ppd.AssistedDelivery).HasDefaultValue(false);
            builder.Property(ppd => ppd.StillbirthOrMultipleDeaths).HasDefaultValue(false);

            builder.HasIndex(ppd => ppd.PregnancyDetailId);

            builder.HasOne(ppd => ppd.PregnancyDetails)
                .WithOne(pd => pd.PreviousPregnancyDelivaryComplications)
                .HasForeignKey<PreviousPregnancyDeliveryComplications>(ppd => ppd.PregnancyDetailId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
