using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config.Maternal
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
                .WithOne(pd => pd.PreviousPregnancyDeliveryComplications)
                .HasForeignKey<PreviousPregnancyDeliveryComplications>(ppd => ppd.PregnancyDetailId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
