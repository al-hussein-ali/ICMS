using ICMS.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config
{
    public class PreviousPregnancyComplicationsConfig : IEntityTypeConfiguration<PreviousPregnancyComplications>
    {
        public void Configure(EntityTypeBuilder<PreviousPregnancyComplications> builder)
        {
            builder.HasKey(ppc => ppc.Id);

            builder.Property(ppc => ppc.Id).HasColumnName("PreviousPregnancyComplicationsId");

            builder.Property(ppc => ppc.VaginalBleedingDuringPregnancy).HasDefaultValue(false);
            builder.Property(ppc => ppc.RecurrentMiscarriageMoreThanThree).HasDefaultValue(false);
            builder.Property(ppc => ppc.Diabetes).HasDefaultValue(false);
            builder.Property(ppc => ppc.Epilepsy).HasDefaultValue(false);
            builder.Property(ppc => ppc.HeartDisease).HasDefaultValue(false);
            builder.Property(ppc => ppc.Preeclampsia).HasDefaultValue(false);
            builder.Property(ppc => ppc.PretermBirthBefore8Months).HasDefaultValue(false);

            builder.HasIndex(ppc => ppc.PregnancyDetailId);

            builder.HasOne(ppc => ppc.PregnancyDetails)
                .WithOne(pd => pd.PreviousPregnancyComplications)
                .HasForeignKey<PreviousPregnancyComplications>(ppc => ppc.PregnancyDetailId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
