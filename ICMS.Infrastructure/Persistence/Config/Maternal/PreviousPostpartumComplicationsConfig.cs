using ICMS.Domain.Entites.Maternal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config.Maternal
{
    public class PreviousPostpartumComplicationsConfig : IEntityTypeConfiguration<PreviousPostpartumComplications>
    {
        public void Configure(EntityTypeBuilder<PreviousPostpartumComplications> builder)
        {
            builder.HasKey(ppc => ppc.Id);
            builder.Property(ppc => ppc.Id).HasColumnName("PreviousPostpartumComplicationsId");

            builder.Property(ppc => ppc.VaginalBleeding).HasDefaultValue(false);
            builder.Property(ppc => ppc.PlacentaRetention).HasDefaultValue(false);
            builder.Property(ppc => ppc.VaginalFistula).HasDefaultValue(false);
            builder.Property(ppc => ppc.PuerperalSepsis).HasDefaultValue(false);
            builder.Property(ppc => ppc.NeonatalDeathWithinFirstWeek).HasDefaultValue(false);

            builder.HasIndex(ppc => ppc.PregnancyDetailId);

            builder.HasOne(ppc => ppc.PregnancyDetails)
                .WithOne(pd => pd.PreviousPostpartumComplications)
                .HasForeignKey<PreviousPostpartumComplications>(ppc => ppc.PregnancyDetailId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
