using ICMS.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config
{
    public class PreviousPostartumComplicationsConfig : IEntityTypeConfiguration<PreviousPostartumComplications>
    {
        public void Configure(EntityTypeBuilder<PreviousPostartumComplications> builder)
        {
            builder.HasKey(ppc => ppc.Id);
            builder.Property(ppc => ppc.Id).HasColumnName("PreviousPostartumComplicationsId");

            builder.Property(ppc => ppc.VaginalBleeding).HasDefaultValue(false);
            builder.Property(ppc => ppc.PlacentaRetention).HasDefaultValue(false);
            builder.Property(ppc => ppc.VaginalFistula).HasDefaultValue(false);
            builder.Property(ppc => ppc.PuerperalSepsis).HasDefaultValue(false);
            builder.Property(ppc => ppc.NeonatalDeathWithinFirstWeek).HasDefaultValue(false);

            builder.HasIndex(ppc => ppc.PregnancyDetailId);

            builder.HasOne(ppc => ppc.PregnancyDetails)
                .WithOne(pd => pd.PreviousPostartumComplications)
                .HasForeignKey<PreviousPostartumComplications>(ppc => ppc.PregnancyDetailId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
